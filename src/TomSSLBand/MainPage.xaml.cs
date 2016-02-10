using Microsoft.Band;
using Microsoft.Band.Notifications;
using Microsoft.Band.Sensors;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TomSSLBand
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        IBandInfo[] pairedBands;
        IBandClient bandClient;
        bool busy;
        bool active;

        public MainPage()
        {
            this.InitializeComponent();
            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            busy = true;
            pairedBands = await BandClientManager.Instance.GetBandsAsync();
            try
            {
                bandClient = await BandClientManager.Instance.ConnectAsync(pairedBands[0]);
                var fwVersion = await bandClient.GetFirmwareVersionAsync();
                var hwVersion = await bandClient.GetHardwareVersionAsync();
                Sensor1Display.Text = fwVersion + " " + hwVersion + " R"; // this information will get wiped out when we start getting the heart rate data.
                GetHeartRateInfo(bandClient); // need to ensure we have user consent, so this is slightly more involved than the other sensor subscriptions.
                GetSensorInfo(bandClient.SensorManager.Accelerometer, Sensor2Display);
                GetSensorInfo(bandClient.SensorManager.SkinTemperature, Sensor3Display);
                GetSensorInfo(bandClient.SensorManager.AmbientLight, Sensor4Display);
                GetSensorInfo(bandClient.SensorManager.Barometer, Sensor5Display);
                GetSensorInfo(bandClient.SensorManager.Gsr, Sensor6Display);
                busy = false;
                active = true;
            }
            catch (BandException x)
            {
                // Do something sensible
            }
        }

        private async void GetHeartRateInfo(IBandClient bandClient)
        {
            // Check if the current user has given consent to the collection of heart rate sensor data.
            if (bandClient.SensorManager.HeartRate.GetCurrentUserConsent() !=
            UserConsent.Granted)
            {
                // We don't have user consent, so let's request it.
                await bandClient.SensorManager.HeartRate.RequestUserConsentAsync();
            }

          
            GetSensorInfo(bandClient.SensorManager.HeartRate, Sensor1Display);
        }

        private async void GetSensorInfo<T>(IBandSensor<T> sensor, TextBlock textBlock) where T : IBandSensorReading
        {
            sensor.ReadingChanged += async (sender, args) =>
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    textBlock.Text = FormatSensorReading[typeof(T)].Invoke(args.SensorReading);
                });
            };
            try
            {
                await sensor.StartReadingsAsync();
            }
            catch (BandException ex)
            {
                textBlock.Text = "It went wrong!";
            }
        }

        Dictionary<Type, Func<IBandSensorReading, string>> FormatSensorReading = new Dictionary<Type, Func<IBandSensorReading, string>>
        {
            {typeof(IBandAccelerometerReading), bsr => string.Format("{0} - {1} - {2}", Math.Round(((IBandAccelerometerReading)bsr).AccelerationX, 2), Math.Round(((IBandAccelerometerReading)bsr).AccelerationY, 2), Math.Round(((IBandAccelerometerReading)bsr).AccelerationZ, 2))},
            {typeof(IBandAltimeterReading), bsr => string.Format("{0} - {1}", ((IBandAltimeterReading)bsr).FlightsAscended, ((IBandAltimeterReading)bsr).FlightsDescended)},
            {typeof(IBandBarometerReading), bsr => string.Format("{0} - {1}", Math.Round(((IBandBarometerReading)bsr).Temperature, 2), Math.Round(((IBandBarometerReading)bsr).AirPressure, 2))},
            {typeof(IBandHeartRateReading), bsr => string.Format("{0} ({1})", ((IBandHeartRateReading)bsr).HeartRate, ((IBandHeartRateReading)bsr).Quality)},
            {typeof(IBandAmbientLightReading), bsr => string.Format("{0} lux", ((IBandAmbientLightReading)bsr).Brightness) },
            {typeof(IBandGsrReading), bsr => string.Format("{0}", ((IBandGsrReading)bsr).Resistance) },
            {typeof(IBandSkinTemperatureReading), bsr => string.Format("{0}", Math.Round(((IBandSkinTemperatureReading)bsr).Temperature, 2)) }
        };

        protected override async void OnTapped(TappedRoutedEventArgs e)
        {
            base.OnTapped(e);
            // Tap the screen to toggle taking readings
            try
            {
                if (!busy)
                {
                    if (active)
                    {
                        // stop everything
                        await bandClient.SensorManager.HeartRate.StopReadingsAsync();
                        await bandClient.SensorManager.Barometer.StopReadingsAsync();
                        await bandClient.SensorManager.AmbientLight.StopReadingsAsync();
                        await bandClient.SensorManager.Accelerometer.StopReadingsAsync();
                        await bandClient.SensorManager.Gsr.StopReadingsAsync();
                        await bandClient.SensorManager.SkinTemperature.StopReadingsAsync();
                        await bandClient.NotificationManager.VibrateAsync(VibrationType.RampDown);
                        Sensor1Display.Text = "---";
                        Sensor2Display.Text = "---";
                        Sensor3Display.Text = "---";
                        Sensor4Display.Text = "---";
                        Sensor5Display.Text = "---";
                        Sensor6Display.Text = "---";
                        active = false;
                    }
                    else
                    {
                        // start everything
                        await bandClient.NotificationManager.VibrateAsync(VibrationType.RampUp);
                        await bandClient.SensorManager.HeartRate.StartReadingsAsync();
                        await bandClient.SensorManager.Accelerometer.StartReadingsAsync();
                        await bandClient.SensorManager.AmbientLight.StartReadingsAsync();
                        await bandClient.SensorManager.Barometer.StartReadingsAsync();
                        await bandClient.SensorManager.Gsr.StartReadingsAsync();
                        await bandClient.SensorManager.SkinTemperature.StartReadingsAsync();
                        active = true;
                    }
                }
            }
            catch (BandException ex)
            {
                // handle a Band connection exception
            }
        }
    }
}
