namespace Photon.Voice.Unity
{
    using UnityEngine;
    using Photon.WebGl.Plugin;

    /// <summary>A wrapper around UnityEngine.Microphone to be able to safely use Microphone and compile for WebGL.</summary>
    public static class UnityMicrophone
    {
        public static string[] devices
        {
            get
            {
                #if UNITY_WEBGL
                if(!CustomMicrophone.HasMicrophonePermission())
                {
                    CustomMicrophone.RequestMicrophonePermission();
                }
                if(CustomMicrophone.devices.Length < 1)
                {
                    CustomMicrophone.RefreshMicrophoneDevices();
                }
                return CustomMicrophone.devices;
                #else
                return Microphone.devices;
                #endif
            }
        }

        public static void End(string deviceName)
        {
            #if UNITY_WEBGL
            CustomMicrophone.End(deviceName);
            #else
            Microphone.End(deviceName);
            #endif
        }
        
        public static void GetDeviceCaps(string deviceName, out int minFreq, out int maxFreq)
        {
            #if UNITY_WEBGL
            CustomMicrophone.GetDeviceCaps(deviceName, out minFreq, out maxFreq);
            #else
            Microphone.GetDeviceCaps(deviceName, out minFreq, out maxFreq);
            #endif
        }

        public static int GetPosition(string deviceName)
        {
            #if UNITY_WEBGL
            return CustomMicrophone.GetPosition(deviceName);
            #else
            return Microphone.GetPosition(deviceName);
            #endif
        }

        public static bool IsRecording(string deviceName)
        {
            #if UNITY_WEBGL
            return CustomMicrophone.IsRecording(deviceName);
            #else
            return Microphone.IsRecording(deviceName);
            #endif
        }

        public static AudioClip Start(string deviceName, bool loop, int lengthSec, int frequency)
        {
            #if UNITY_WEBGL
            return CustomMicrophone.Start(deviceName, loop, lengthSec, frequency);
            #else
            return Microphone.Start(deviceName, loop, lengthSec, frequency);
            #endif
        }
    }
}