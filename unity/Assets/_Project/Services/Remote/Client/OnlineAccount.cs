using System;
using System.IO;
using UnityEngine;

namespace Steel.Services.Remote
{
    /// <summary>The server address and account token last used on this device, kept in the app's own folder so the
    /// player does not type 43 characters at every start.</summary>
    [Serializable] public sealed class OnlineAccount
    {
        private const string FileName = "akun-online.json";
        public string origin = "", token = "";
        public bool Known => !string.IsNullOrEmpty(token);

        public CareerClient Client() => new CareerClient(CareerEndpoint.ForHost(origin), token);

        public static OnlineAccount Read(string folder)
        {
            try
            {
                string path = Path.Combine(folder, FileName);
                return File.Exists(path) ? JsonUtility.FromJson<OnlineAccount>(File.ReadAllText(path)) ?? new OnlineAccount()
                                         : new OnlineAccount();
            }
            catch (Exception broken) when (broken is IOException || broken is ArgumentException || broken is UnauthorizedAccessException)
            { return new OnlineAccount(); }
        }

        /// <summary>False when the device would not keep it; the player then signs in again next time.</summary>
        public bool Write(string folder)
        {
            try { File.WriteAllText(Path.Combine(folder, FileName), JsonUtility.ToJson(this)); return true; }
            catch (Exception broken) when (broken is IOException || broken is UnauthorizedAccessException) { return false; }
        }

        public static void Forget(string folder)
        {
            try { File.Delete(Path.Combine(folder, FileName)); }
            catch (Exception broken) when (broken is IOException || broken is UnauthorizedAccessException) { }
        }
    }
}
