using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer_Test.Classes
{
    public class UserClass
    {
        public string userName { get; set; }
        public int userID { get; set; }
        public string? nickName { get; set; }
        public string password { get; set; }
        public bool isBanned { get; private set; }
        public bool isMuted { get; private set; }
        public bool isDeafed { get; private set; }
        //byte[] key { get; set; }
        //byte[] iv { get; set; }
        public DateTime registrationDate { get; set; }
        public IPEndPoint associatedIPaddres { get; set; }

        public UserClass()
        {
            this.userName = "#null";
        }

        public UserClass(string userName, string password, DateTime registrationDate, string nickName)
        {
            this.userName = userName;
            this.password = password;
            this.registrationDate = registrationDate;
            this.nickName = nickName;
            //using (var gen = new RNGCryptoServiceProvider())
            //{
            //    gen.GetBytes(key);
            //    gen.GetBytes(iv);
            //}
            isBanned = false;
            isMuted = false;
            isDeafed = false;
        }

        public void BanUser(string userName)
        {
            throw new NotImplementedException();
        }

        public void UnBanUser(string userName)
        {
            throw new NotImplementedException();
        }

        public void ServerMuteUserMicrophone(string userName)
        {
            throw new NotImplementedException();
        }

        public void ServerUnMuteUserMicrophone(string userName)
        {
            throw new NotImplementedException();
        }

        public void ServerDeafUserMicrophone(string userName)
        {
            throw new NotImplementedException();
        }

        public void ServerUnDeafUserMicrophone(string userName)
        {
            throw new NotImplementedException();
        }

        public void DeleteUser(string userName)
        {

        }
    }
}
