using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using mjpeg;
using XFiresecAPI;

namespace VideoModule.ViewModels
{
	public class CameraViewModel : BaseViewModel
	{
		public Camera Camera { get; set; }
		private const int IMAGES_BUFFER_SIZE = 10;
		private int ImagesBufferIndex = 0;
		private bool VideoThreadInterrupt = false;
		public CameraViewModel(Camera camera)
		{
			Camera = camera;
			StartVideoCommand = new RelayCommand(OnStartVideo);
			PauseVideoCommand = new RelayCommand(OnPauseVideo);
			StopVideoCommand = new RelayCommand(OnStopVideo);
			ImagesBuffer = new List<Bitmap>(IMAGES_BUFFER_SIZE);
			InitializeImagesBuffer();
		}
		List<Bitmap> ImagesBuffer { get; set; }
		void InitializeImagesBuffer()
		{
			for (int i = 0; i < IMAGES_BUFFER_SIZE; i++)
				ImagesBuffer.Add(new Bitmap(100, 100));
		}
		public string PresenrationZones
		{
			get
			{
				var zones = new List<XZone>();
				foreach (var zoneUID in Camera.ZoneUIDs)
				{
					var zone = XManager.Zones.FirstOrDefault(x => x.UID == zoneUID);
					if (zone != null)
						zones.Add(zone);
				}
				var presentationZones = XManager.GetCommaSeparatedZones(zones);
				return presentationZones;
			}
		}

		public void Update()
		{
			OnPropertyChanged("Camera");
			OnPropertyChanged("PresentationZones");
		}

		Thread videoThread { get; set; }
		public RelayCommand StartVideoCommand { get; private set; }
		void OnStartVideo()
		{
			VideoThreadInterrupt = false;
			videoThread = new Thread(StartVideo);
			videoThread.Start();
		}
		public RelayCommand PauseVideoCommand { get; set; }
		void OnPauseVideo()
		{
			VideoThreadInterrupt = true;
		}
		public RelayCommand StopVideoCommand { get; set; }
		void OnStopVideo()
		{
			StopVideo();
		}

		public void StopVideo()
		{
			VideoThreadInterrupt = true;
			ImageSource = new BitmapImage();

		}
		void StartVideo()
		{
			string source = "http://"+ Camera.Address +"/cgi-bin/mjpg/video.cgi";
			string login = "admin";
			string password = "admin";
			int framesReceived = 0;
			int bytesReceived = 0;
			bool useSeparateConnectionGroup = true;
			const int bufSize = 512 * 1024; // buffer size
			const int readSize = 1024; // portion size to read
			byte[] buffer = new byte[bufSize]; // buffer to read stream

			while (true)
			{
				HttpWebRequest req = null;
				WebResponse resp = null;
				Stream stream = null;
				byte[] delimiter = null;
				byte[] delimiter2 = null;
				byte[] boundary = null;
				int boundaryLen, delimiterLen = 0, delimiter2Len = 0;
				int read, todo = 0, total = 0, pos = 0, align = 1;
				int start = 0, stop = 0;

				// align
				//  1 = searching for image start
				//  2 = searching for image end
				try
				{
					// create request
					req = (HttpWebRequest)WebRequest.Create(source);
					// set login and password
					if ((login != null) && (password != null) && (login != ""))
						req.Credentials = new NetworkCredential(login, password);
					// set connection group name
					if (useSeparateConnectionGroup)
						req.ConnectionGroupName = GetHashCode().ToString();
					// get response
					resp = req.GetResponse();

					// check content type
					string ct = resp.ContentType;
					if (ct.IndexOf("multipart/x-mixed-replace") == -1)
						throw new ApplicationException("Invalid URL");

					// get boundary
					ASCIIEncoding encoding = new ASCIIEncoding();
					boundary = encoding.GetBytes(ct.Substring(ct.IndexOf("boundary=", 0) + 9));
					boundaryLen = boundary.Length;

					// get response stream
					stream = resp.GetResponseStream();

					// loop
					while (true)
					{
						if (VideoThreadInterrupt)
							return;
						// check total read
						if (total > bufSize - readSize)
						{
							total = pos = todo = 0;
						}

						// read next portion from stream
						if ((read = stream.Read(buffer, total, readSize)) == 0)
							throw new ApplicationException();

						total += read;
						todo += read;

						// increment received bytes counter
						bytesReceived += read;

						// does we know the delimiter ?
						if (delimiter == null)
						{
							// find boundary
							pos = ByteArrayUtils.Find(buffer, boundary, pos, todo);

							if (pos == -1)
							{
								// was not found
								todo = boundaryLen - 1;
								pos = total - todo;
								continue;
							}

							todo = total - pos;

							if (todo < 2)
								continue;

							// check new line delimiter type
							if (buffer[pos + boundaryLen] == 10)
							{
								delimiterLen = 2;
								delimiter = new byte[2] { 10, 10 };
								delimiter2Len = 1;
								delimiter2 = new byte[1] { 10 };
							}
							else
							{
								delimiterLen = 4;
								delimiter = new byte[4] { 13, 10, 13, 10 };
								delimiter2Len = 2;
								delimiter2 = new byte[2] { 13, 10 };
							}

							pos += boundaryLen + delimiter2Len;
							todo = total - pos;
						}

						// search for image
						if (align == 1)
						{
							start = ByteArrayUtils.Find(buffer, delimiter, pos, todo);
							if (start != -1)
							{
								// found delimiter
								start += delimiterLen;
								pos = start;
								todo = total - pos;
								align = 2;
							}
							else
							{
								// delimiter not found
								todo = delimiterLen - 1;
								pos = total - todo;
							}
						}

						// search for image end
						while ((align == 2) && (todo >= boundaryLen))
						{
							stop = ByteArrayUtils.Find(buffer, boundary, pos, todo);
							if (stop != -1)
							{
								pos = stop;
								todo = total - pos;

								// increment frames counter
								framesReceived++;

								// image at stop
								//if (NewFrame != null)
								if (start != 0x32)
								{
									var bmp = (Bitmap)Bitmap.FromStream(new MemoryStream(buffer, start, stop - start));
									ImagesBufferIndex = ImagesBufferIndex%IMAGES_BUFFER_SIZE;
									ImagesBuffer[ImagesBufferIndex] = new Bitmap(bmp);
									ImagesBufferIndex++;
									using (var memory = new MemoryStream())
									{
										bmp.Save(memory, ImageFormat.Jpeg);
										memory.Position = 0;
										var bitmapImage = new BitmapImage();
										bitmapImage.BeginInit();
										bitmapImage.StreamSource = memory;
										bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
										bitmapImage.EndInit();
										bitmapImage.Freeze();
										Dispatcher.CurrentDispatcher.Invoke(new Action(() =>
										{
											ImageSource = bitmapImage;
										}));
									}

									//bmp.Save("c:\\1111.jpeg");
									// notify client
									//NewFrame(this, new CameraEventArgs(bmp));
									// release the image
									bmp.Dispose();
									bmp = null;
								}

								// shift array
								pos = stop + boundaryLen;
								todo = total - pos;
								Array.Copy(buffer, pos, buffer, 0, todo);

								total = todo;
								pos = 0;
								align = 1;
							}
							else
							{
								// delimiter not found
								todo = boundaryLen - 1;
								pos = total - todo;
							}
						}
					}
				}
				catch (WebException ex)
				{
					System.Diagnostics.Debug.WriteLine("=============: " + ex.Message);
					// wait for a while before the next try
					Thread.Sleep(250);
				}
				catch (ApplicationException ex)
				{
					System.Diagnostics.Debug.WriteLine("=============: " + ex.Message);
					// wait for a while before the next try
					Thread.Sleep(250);
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine("=============: " + ex.Message);
				}
				finally
				{
					VideoThreadInterrupt = false;
					// abort request
					if (req != null)
						req.Abort();
					// close response stream
					if (stream != null)
						stream.Close();
					// close response
					if (resp != null)
						resp.Close();
				}
			}
		}
		private ImageSource _imageSource;
		public ImageSource ImageSource
		{
			get { return _imageSource; }
			set
			{
				_imageSource = value;
				OnPropertyChanged("ImageSource");
			}
		}
	}
}