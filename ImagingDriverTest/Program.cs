using SeniorDesignFall2024.ImagingDriver.SDCam;

namespace ImagingDriverTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SDCamController c = new SDCamController();
            c.Test();
        }
    }
}
