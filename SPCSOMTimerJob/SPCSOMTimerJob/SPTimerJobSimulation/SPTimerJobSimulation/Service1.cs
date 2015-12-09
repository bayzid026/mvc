using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;

namespace SPTimerJobSimulation
{
    public partial class Service1 : ServiceBase
    {
        private static string SharePointPrincipal = "00000003-0000-0ff1-ce00-000000000000";
        static Timer SPtimer;
        
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            SPtimer = new Timer();
            //schedule an interval of 2 minutes
            SPtimer.Interval = 1000 * 60 * 2;
            SPtimer.Elapsed += SPtimer_Elapsed;
            startSPtimer();
            
        }

        void SPtimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //Comment out either of the below based on On Premise server or Online
           // Uri hostWeb = new Uri("http://tenant/sites/DevCenter");

            //Below line works with On line
            Uri hostWeb = new Uri("https://sweethome03.sharepoint.com");

            string realm = TokenHelper.GetRealmFromTargetUrl(hostWeb);

            string appOnlyAccessToken = TokenHelper.GetAppOnlyAccessToken(SharePointPrincipal, hostWeb.Authority, realm).AccessToken;

            using (ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(hostWeb.ToString(), appOnlyAccessToken))
            {
                if (clientContext != null)
                {
                    var myList = clientContext.Web.Lists.GetByTitle("WindowsTimerJob");
                    ListItemCreationInformation listItemCreate = new ListItemCreationInformation();
                    Microsoft.SharePoint.Client.ListItem newItem = myList.AddItem(listItemCreate);
                    newItem["Title"] = "Added from Timer Job";
                    newItem.Update();
                    clientContext.ExecuteQuery();
                }
            }
        }

        private static void startSPtimer()
        {
            SPtimer.Start();
        }

        protected override void OnStop()
        {
        }
    }
}
