using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CESDK;
using DiscordRPC;
using System.Windows.Forms;
using System.IO;

namespace cheatengine_rpc
{
    class Program : CESDKPluginClass
    {
        public override string GetPluginName()
        {
            return "Cheat Engine Discord RPC by Lynxz Gaming, github : adhptrh/cheatengine-rpc";
        }

        public override bool DisablePlugin()
        {
            return true;
        }

        int Update()
        {
            string process = sdk.lua.ToString(-4) ?? "nothing";
            string path = Path.GetFileName(sdk.lua.ToString(-3));
            string ceversion = sdk.lua.ToString(-2);
            string addresslist = sdk.lua.ToNumber(-1).ToString();

            rpcAssets.LargeImageText = "CE Version: " + ceversion;
            rpcAssets.SmallImageText = "Address List : " + addresslist;
            
            rpc.Details = "Attaching " + process;
            rpc.State = "CT: " + path;
            rpc.Assets = rpcAssets;

            drc.SetPresence(rpc);
            return 1;
        }
        
        DiscordRpcClient drc;
        RichPresence rpc;
        Assets rpcAssets;

        public override bool EnablePlugin()
        {
            sdk.lua.Register("UpdateRPC", Update);

            drc = new DiscordRpcClient("739213592410456136");
            drc.Initialize();

            rpc = new RichPresence();
            rpc.Details = "Just opened";

            rpc.Timestamps = new Timestamps()
            {
                StartUnixMilliseconds = (ulong)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds
            };

            rpcAssets = new Assets()
            {
                LargeImageKey = "ce2",
                SmallImageKey = "ce2",
            };

            Timer tmr = new Timer();
            tmr.Enabled = true;
            tmr.Interval = 1000;
            tmr.Tick += Tmr_Tick;

            return true;
        }

        private void Tmr_Tick(object sender, EventArgs e)
        {
            GC.Collect();
            sdk.lua.DoString(@"
UpdateRPC(
  process,
  (MainForm.OpenDialog1.filename and MainForm.OpenDialog1.filename:find(':')) and 
    MainForm.OpenDialog1.filename or
  (MainForm.SaveDialog1.filename and MainForm.SaveDialog1.filename:find(':')) and 
    MainForm.SaveDialog1.filename or
  'None',
  tostring(getCEVersion()),
  getAddressList().count
)");
        }
    }
}
