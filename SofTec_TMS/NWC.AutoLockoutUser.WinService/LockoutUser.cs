using NLog;
using NWC.DAL.NWCEntities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace NWC.AutoLockoutUser.WinService
{
    public class LockoutUser
    {
        #region prop
        private readonly bool LockoutUserEnable;


        private System.Timers.Timer _timer;
        private Thread LockoutUserThread;

       
        private readonly string _token;
        
        #endregion prop

        #region ctor

        public LockoutUser()
        {
            try
            {
                this.LockoutUserEnable = KeyConfig.LockoutUserEnable;
                var jobInterval = KeyConfig.WindowsServiceTimerIntervalMinutes;
                _timer = new System.Timers.Timer(1000 * 60 * jobInterval) { AutoReset = true };
                _timer.Elapsed += new ElapsedEventHandler(ServiceTimer_Tick);
                // //var loginDto = new AuthenticationManager().AuthenticateUser(authenticationAPI_URL, username, password);
                //// _token = loginDto.Value != null && !string.IsNullOrEmpty(loginDto.Value.Token) ? loginDto.Value.Token : null;
                // if (_token != null)
                // {         

                //     //To set job interval in Minutes
                //     var jobInterval = KeyConfig.WindowsServiceTimerIntervalMinutes;
                //     _timer = new System.Timers.Timer(1000 * 60 * jobInterval) { AutoReset = true };
                //     _timer.Elapsed += new ElapsedEventHandler(ServiceTimer_Tick);

                //     RegisterLog(LogLevel.Info, "***** NWC.AutoLockoutUser.WinService Started At " + DateTime.Now);
                // }
                // else
                // {
                //     RegisterLog(LogLevel.Error, "***** NWC.AutoLockoutUser.WinService Login failed At " + DateTime.Now);
                //     throw new Exception("Login failed.");
                // }
            }
            catch (Exception ex)
            {
                RegisterLog(LogLevel.Error, "***** Error At  :" + DateTime.Now + " - " + ex.Message);
            }
        }

        #endregion ctor

        #region Timer Event and Setting

        public void Start()
        {
            _timer.Start();
            ServiceTimer_Tick(null, null);
        }

        public void Stop()
        {
            _timer.Stop();
        }

        #region Setting

        public void RegisterLog(LogLevel level, string message)
        {
            LogManager.GetLogger("NWC.AutoLockoutUser.WinService.WinService").Log(level, message);
        }

        public void ServiceTimer_Tick(object sender, ElapsedEventArgs e)
        {
            //RegisterLog(LogLevel.Info, "***** Timer Tick Started At " + DateTime.Now);

            if (LockoutUserEnable && (LockoutUserThread == null || !LockoutUserThread.IsAlive))
            {
                LockoutUserThread = new Thread(new ThreadStart(LockoutUserThreadMethod));
                LockoutUserThread.Start();
            }

        }

        #endregion Setting


        public void LockoutUserThreadMethod()
        {
            try
            {
                RegisterLog(LogLevel.Info, "|--- Started At " + DateTime.Now + " --  Call LockoutUserThreadMethod");
                NWCContext db = new NWCContext();
                var users = db.aspnet_Membership.Where(a => a.IsLockedOut).ToList();
                if (users != null && users.Count > 0)
                {
                    users.ForEach(u => {
                        RegisterLog(LogLevel.Info, " UnLock  User : " + u.aspnet_Users.UserName + "");
                        //Debug.WriteLine("UnLock  User : " + u.aspnet_Users.UserName + "");
                        u.IsLockedOut = false;
                        u.FailedPasswordAttemptCount = 0;
                        }) ;
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                RegisterLog(LogLevel.Error, "***** Error :" + ex.Message);
            }
            RegisterLog(LogLevel.Info, "|--- Ended At " + DateTime.Now + " -- Call LockoutUserThreadMethod");
        }
        #endregion Timer Event and Setting
    }
}
