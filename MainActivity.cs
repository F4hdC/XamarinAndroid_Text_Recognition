using Android;
using Android.App;
using Android.Gms.Vision;
using Android.Gms.Vision.Texts;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.App;
using System;
using Android.Content.PM;
using System.Drawing;
using System.Text;
using Java.Util.Concurrent;


namespace LectureAppAndroid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity,ISurfaceHolderCallback, Detector.IProcessor, Java.Lang.IRunnable
    {
        SurfaceView MyCamerView;
        TextView MyTxtView;
        CameraSource MyCameraSource;
        Button MyBtnStart;
        TextRecognizer MyTextRecognizer;
        SparseArray items;
        Canvas MyCanvas = new Canvas();


        private static int RequestPermissionId = 101;
        public static int MyCheckPermission = 10;
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (grantResults[0] == Permission.Granted)
            {
                try
                {
                    if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.Camera) == Permission.Granted)
                    {
                        MyCameraSource.Start(MyCamerView.Holder);
                    }

                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("OnRequestPermissionsResult: " + e.Message);
                }
            }

        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            MyCamerView = FindViewById<SurfaceView>(Resource.Id.surfaceView);
            MyTxtView = FindViewById<TextView>(Resource.Id.text_view);
            MyBtnStart = FindViewById<Button>(Resource.Id.btn_start);
            MyBtnStart.Click += Btn_Click;
        }
        
        private void Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if ((MyBtnStart.Text == "Start"))
                {
                    StartCameraSource();
                    MyBtnStart.Text = "Stop";

                }
                else
                {
                    MyBtnStart.Text = "Start";
                    MyCameraSource.Stop();
                    MyTextRecognizer = null;

                }
            } catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Btn_Click: " +ex.Message);
            }

            
            
        }

        private void StartCameraSource()
        {
            try
            {
                MyTextRecognizer = new TextRecognizer.Builder(this).Build();
                if (!MyTextRecognizer.IsOperational)
                {
                    System.Diagnostics.Debug.WriteLine("Detector dependencies not loaded yet");
                }
                else
                {
                    MyCameraSource = new CameraSource.Builder(this, MyTextRecognizer)
                        .SetFacing(CameraFacing.Back)
                        .SetRequestedPreviewSize(1280, 1024)
                        .SetAutoFocusEnabled(true)
                        .SetRequestedFps(2.0f)
                        .Build();
                    MyCamerView.Holder.AddCallback(this);
                    MyTextRecognizer.SetProcessor(this);

                    if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.Camera) == Permission.Granted)
                    {
                        MyCameraSource.Start(MyCamerView.Holder);

                    }
                    else
                    {
                        ActivityCompat.RequestPermissions(this,
                                new String[] { Manifest.Permission.Camera },
                                RequestPermissionId);
                    }
                }
            }
            catch (Exception ex1)
            {
                System.Diagnostics.Debug.WriteLine("StartCameraSource: " + ex1.Message);
            }
        }

        

        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {
            

        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            try
            {
                if(ActivityCompat.CheckSelfPermission(this,Manifest.Permission.Camera) != Android.Content.PM.Permission.Granted)
                {
                    ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.Camera }, RequestPermissionId);
                    return;
                }
                MyCameraSource.Start(MyCamerView.Holder);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("SurfaceCreated: " + e.Message);
            }

            //drawrect();
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
           // MyCameraSource.Stop();
        }

        public void ReceiveDetections(Detector.Detections detections)
        {
            try
            {
                items = detections.DetectedItems;
                if (items.Size() != 0)
                {
                    MyTxtView.Post(this);
                }
            }
            catch (Exception ex2)
            {
                System.Diagnostics.Debug.WriteLine("ReceiveDetections: " + ex2.Message);
            }
        }

        public void Release()
        {
            //throw new NotImplementedException();
        }

        public void Run()
        {
            try
            {
               // string MyBook = "DOS MUJERES Y UN HOMBRE";
               // bool Found = false;

                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < items.Size(); i++)
                {
                   // var item = items.ValueAt(i) as TextBlock;
                    stringBuilder.Append(((TextBlock)items.ValueAt(i)).Value);
                    stringBuilder.Append("\n");

                    //Intenté debujar un rect con las coordinadas de Bounding 
                    /*
                    Paint MyPaint = new Paint();
                    MyRect = new Rect(item.BoundingBox);
                    MyRect = item.BoundingBox;

                    MyPaint.SetARGB(23, 33, 45, 12);
                    MyCanvas.DrawRect(MyRect, MyPaint);

                    
                   // MyCamerView.ClipBounds = MyRect; */


                }
                MyTxtView.Text = stringBuilder.ToString();
            }
            catch (Exception ex)
            {
                Log.Error(ToString(), ex.Message);
            }
         
        }

      
    }

}