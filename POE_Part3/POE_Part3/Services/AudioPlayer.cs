using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;

using System.Threading.Tasks;
using System.Windows;

namespace Part_2.Services;

public class AudioPlayer
{
  SoundPlayer player;
    public AudioPlayer()
    {
        player = new SoundPlayer("Assets/Greeting.wav");
    }
    public void PlayNotification()
    {
        try
        {
            player.Play();
        }
        catch (Exception ex)
        {
           MessageBox.Show($"Error playing sound: {ex.Message}");
            Console.WriteLine($"Error playing sound: {ex.Message}");
        }
    }
}