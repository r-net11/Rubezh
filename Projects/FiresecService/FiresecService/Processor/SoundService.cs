using System;
using System.Media;

namespace FiresecService
{
    public static class SoundService
    {
        // Sound1.wav

        public static void Play(string sound)
        {
            SoundPlayer sp = new SoundPlayer(@"C:\Program Files\Firesec\Firesec\Sounds\" + sound);
            sp.Play();
        }

        public static void Beep()
        {
            Console.Beep(2000, 1000);
        }
    }
}
