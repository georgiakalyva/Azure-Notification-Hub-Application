using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Azure.NotificationHubs;

namespace AzureNotificationsApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string hub = "tapnorderhub";
        public string connectionstring = "Endpoint=sb://tapnorderhub-ns.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=0jOCeQzT4Xtj9xkCnvT0bJ7nYWOgIGpNjDArTmms+hc=";
        public MainWindow()
        {
            InitializeComponent(); 
            iOSSend.Click += iOSSend_Click;
            InitUI();
          
        }

        async void InitUI()
         {
            String hubname = "tapnorderhub";
            String connectionstring = "Endpoint=sb://tapnorderhub-ns.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=0jOCeQzT4Xtj9xkCnvT0bJ7nYWOgIGpNjDArTmms+hc=";

            NotificationHubClient hub = NotificationHubClient.CreateClientFromConnectionString(connectionstring, hubname, iOSTest.IsChecked.Value);
                
             var GetAllRegistrations = await hub.GetAllRegistrationsAsync(1000);

             ActiveDevicesTxb.Text = "Active Devices: "+
                GetAllRegistrations.OfType<AppleTemplateRegistrationDescription>().Count()+" in iOS and "+
                GetAllRegistrations.OfType<GcmTemplateRegistrationDescription>().Count() + " in Android.";
             List<String> TagsTxb = new List<String>();
             foreach (var Registration in GetAllRegistrations)
             {
                 if (Registration.Tags!=null)
                 {                     
                     foreach (string tag in Registration.Tags)
                     {
                         if (!TagsTxb.Contains(tag))
                         {
                             TagsTxb.Add(tag);
                         }
                     }
                 }
             }

             TagsTb.Text = "Active Tags: ";
             foreach (var tagtext in TagsTxb)
             {
                 TagsTb.Text += tagtext + "  ";
             }
        }

        async void iOSSend_Click(object sender, RoutedEventArgs e)
        {
            /*String data = "{ \"aps\" : {\"alert\":\"";
            if (iOSTxb.Text != "")
            {
             
            data += iOSTxb.Text;
            data += "\"}}";
             
            OutputText.Text += System.Environment.NewLine + "[" + DateTime.Now + "] " + "iOS sending...: " + data;*/
            if (GreekTxb.Text!="" && EnglishTxb.Text!="")
	        {
		 
	
            string Data = @"{""Lang_English"":"""+EnglishTxb.Text+@""", ""Lang_Greek"":"""+GreekTxb.Text+@"""}";

            var notification = new Dictionary<string, string>() {
                        {"Lang_English", EnglishTxb.Text},
                        {"Lang_Greek", GreekTxb.Text}};
            Scrolloutput.ScrollToEnd();
            String hubname = "tapnorderhub";
            String connectionstring = "Endpoint=sb://tapnorderhub-ns.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=0jOCeQzT4Xtj9xkCnvT0bJ7nYWOgIGpNjDArTmms+hc=";

            try
            {
                NotificationHubClient hub = NotificationHubClient.CreateClientFromConnectionString(connectionstring, hubname, iOSTest.IsChecked.Value);
                if (TagsTxb.Text=="")
                {
                    await hub.SendTemplateNotificationAsync(notification);
                }
                else
                {
                    await hub.SendTemplateNotificationAsync(notification, TagsTxb.Text);

                } 
               

                OutputText.Text += System.Environment.NewLine + "[" + DateTime.Now + "] " + "Success";
                Scrolloutput.ScrollToEnd();
            }
            catch (Exception ex)
            {
                OutputText.Text += System.Environment.NewLine+"[" + DateTime.Now + "] " + "Error: " + ex.Message;
                Scrolloutput.ScrollToEnd();
            }

            
                  
            }else
	        {
                OutputText.Text += System.Environment.NewLine + "[" + DateTime.Now + "] " + "Blank notifications wont be sent.";
                Scrolloutput.ScrollToEnd();
                }
}

            private async void DevicesDeleteBtn_Click(object sender, RoutedEventArgs e)
            {
                try
                {

                String hubname = "tapnorderhub";
                String connectionstring = "Endpoint=sb://tapnorderhub-ns.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=0jOCeQzT4Xtj9xkCnvT0bJ7nYWOgIGpNjDArTmms+hc=";

                NotificationHubClient hub = NotificationHubClient.CreateClientFromConnectionString(connectionstring, hubname, iOSTest.IsChecked.Value);
                
                var GetAllRegistrations = await hub.GetAllRegistrationsAsync(1000000);

                ActiveDevicesTxb.Text = "Active Devices: "+
                GetAllRegistrations.OfType<AppleTemplateRegistrationDescription>().Count()+" in iOS and "+
                GetAllRegistrations.OfType<GcmTemplateRegistrationDescription>().Count() + " in Android.";

                if ((bool)iOSRegistrations.IsChecked)
                {
                    foreach (var appleReg in GetAllRegistrations.OfType<AppleTemplateRegistrationDescription>())
                    {
                        await hub.DeleteRegistrationAsync(appleReg);
                    }
                    OutputText.Text += System.Environment.NewLine + "[" + DateTime.Now + "] " + "iOS Registrations Deleted.";
                    Scrolloutput.ScrollToEnd();
                }
                
                if ((bool)AndroidRegistrations.IsChecked)
                {
                    //foreach (var gcmReg in GetAllRegistrations.OfType<GcmTemplateRegistrationDescription>())
                    //{
                    //    await hub.DeleteRegistrationAsync(gcmReg);
                    //}
                    //OutputText.Text += System.Environment.NewLine + "[" + DateTime.Now + "] " + "Android Registrations Deleted.";
                    //Scrolloutput.ScrollToEnd();
                }

                if ((bool)DebugRegistrations.IsChecked)
                {
                    //delete by tags to be safe
                    foreach (var debugReg in GetAllRegistrations.OfType<GcmRegistrationDescription>())
                    {
                        await hub.DeleteRegistrationAsync(debugReg);
                    }
                    OutputText.Text += System.Environment.NewLine + "[" + DateTime.Now + "] " + "Debug Registrations Deleted.";
                    Scrolloutput.ScrollToEnd();
                }

                GetAllRegistrations = await hub.GetAllRegistrationsAsync(1000000);

                ActiveDevicesTxb.Text = "Active Devices: " +
                GetAllRegistrations.OfType<AppleTemplateRegistrationDescription>().Count() + " in iOS and " +
                GetAllRegistrations.OfType<GcmTemplateRegistrationDescription>().Count() + " in Android.";


                }
                catch (Exception ex)
                {
                    OutputText.Text += System.Environment.NewLine + "[" + DateTime.Now + "] " + "Error: " + ex.Message;
                    Scrolloutput.ScrollToEnd();
                }
            }

            

       
                
        
      

        

    }
}
