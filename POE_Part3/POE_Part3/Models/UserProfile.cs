using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Part_2.Models;

public class UserProfile
{
    
        public string GetArt()
        {
            string art = @"
   🤖 CYBERBOT
  ─────────────
  Learn • Protect

  🔐 Passwords
  🎣 Phishing
  🌐 Safe Browsing";
            return art;
        }
        public void PrintArt()
        {
        MessageBox.Show(GetArt());
        }
}