using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Felcon.Extensions
{
    public static class EventHandlerExtensions
    {
        public static void SafeInvoke(this EventHandler<EventArgs> eventHandler , object sender , EventArgs e)
        {
            try
            {
                eventHandler.Invoke(sender, e);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in safe invoke! " + ex.Message);
                //throw ex;
            }
        
        }
        public static void SafeInvoke(this EventHandler<DataEventArgs> eventHandler, object sender, DataEventArgs e)
        {
            try
            {
                eventHandler.Invoke(sender, e);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in safe invoke! " + ex.Message);
                //System.Windows.Forms.MessageBox.Show("Test");
                // throw ex;
            }

        }
    }
}
