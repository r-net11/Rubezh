using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using FiresecAPI.Models;

namespace Infrastructure.Common
{
	public class MjpegCamera
	{
		bool VideoThreadInterrupt = false;
		public string URL { get { return Camera.Address; } }
		public string Login { get { return Camera.Login; } }
		public string Password { get { return Camera.Password; } }
		public Camera Camera { get; private set; }
		public List<StringBuilder> ErrorLog { get; private set; }
		public MjpegCamera(Camera camera)
		{
			Camera = camera;
		}
		
		public event Action<Bitmap> FrameReady;
		public event Action<string> ErrorHandler;
	
		public void StartVideo()
		{
			string source = "http://" + URL + "/cgi-bin/mjpg/video.cgi";
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
					// set Login and Password
					if ((Login != null) && (Password != null) && (Login != ""))
						req.Credentials = new NetworkCredential(Login, Password);
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
							pos = Find(buffer, boundary, pos, todo);
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
							start = Find(buffer, delimiter, pos, todo);
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
							stop = Find(buffer, boundary, pos, todo);
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
									if (VideoThreadInterrupt)
										return;
									var bmp = (Bitmap)Image.FromStream(new MemoryStream(buffer, start, stop - start));
									if (FrameReady != null)
										FrameReady(bmp);
									bmp.Dispose();
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
					ErrorLog.Add(new StringBuilder(ex.Message));
					if (ErrorHandler != null)
						ErrorHandler(ex.Message);
					// wait for a while before the next try
					Thread.Sleep(250);
				}
				catch (ApplicationException ex)
				{
					ErrorLog.Add(new StringBuilder(ex.Message));
					if (ErrorHandler != null)
						ErrorHandler(ex.Message);
					// wait for a while before the next try
					Thread.Sleep(250);
				}
				catch (Exception ex)
				{
					ErrorLog.Add(new StringBuilder(ex.Message));
					if (ErrorHandler != null)
						ErrorHandler(ex.Message);
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

		public void StopVideo()
		{
			VideoThreadInterrupt = true;
		}
		// Find subarray in array
		public static int Find(byte[] array, byte[] needle, int startIndex, int count)
		{
			int needleLen = needle.Length;
			int index;

			while (count >= needleLen)
			{
				index = Array.IndexOf(array, needle[0], startIndex, count - needleLen + 1);

				if (index == -1)
					return -1;

				int i, p;
				// check for needle
				for (i = 0, p = index; i < needleLen; i++, p++)
				{
					if (array[p] != needle[i])
					{
						break;
					}
				}

				if (i == needleLen)
				{
					// found needle
					return index;
				}

				count -= (index - startIndex + 1);
				startIndex = index + 1;
			}
			return -1;
		}
	}
}