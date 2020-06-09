using ApplicationCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Services
{
   public class WorkServer: IWorkServer
    {
        public string GetTime()
        {
            return DateTime.Now.ToString();
        }
    }
}
