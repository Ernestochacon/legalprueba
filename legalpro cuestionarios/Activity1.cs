using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace legalpro_cuestionarios
{
    [Activity(Label = "Legal Pro", ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenSize | Android.Content.PM.ConfigChanges.Orientation, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class Activity1 : Activity
    {
        public static string Legalproprefs = "legalpropref";
        ISharedPreferences opciones;
        ImageButton imageb;
        ImageView pajaralogin;
        TextView textv;
        Android.Views.Animations.Animation animacion;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Mainwindow);
            // Create your application here
            opciones = GetSharedPreferences(Legalproprefs, 0);
            textv = FindViewById<TextView>(Resource.Id.textView3);
            if (opciones.GetString("nm_opr", "").Length == 0)
            {
                Intent intent = new Intent(this.ApplicationContext, typeof(MainActivity));
                StartActivity(intent);
            }
            textv.Text = opciones.GetString("nm_opr", "Error");

            imageb = FindViewById<ImageButton>(Resource.Id.descargar);
            imageb.Click += (sender, e) =>
            {
                Intent intent = new Intent(this.ApplicationContext, typeof(Showmodelos));
                StartActivity(intent);
            };
            animacion = Android.Views.Animations.AnimationUtils.LoadAnimation(this, Resource.Animation.movimientolento);
            animacion.Reset();
            imageb.StartAnimation(animacion);

            imageb = FindViewById<ImageButton>(Resource.Id.lista);
            imageb.Click += (sender, e) =>
            {

                Intent intent = new Intent(this.ApplicationContext, typeof(Cuestionarios_guardados));
                StartActivity(intent);
            };
            animacion = Android.Views.Animations.AnimationUtils.LoadAnimation(this, Resource.Animation.movimientolento5p);
            animacion.Reset();
            imageb.StartAnimation(animacion);

            imageb = FindViewById<ImageButton>(Resource.Id.nuevo);
            imageb.Click += (sender, e) =>
            {

                Intent intent = new Intent(this.ApplicationContext, typeof(Showmodelos));               
                StartActivity(intent);
            };
            animacion = Android.Views.Animations.AnimationUtils.LoadAnimation(this, Resource.Animation.movimientolento);
            animacion.Reset();
            imageb.StartAnimation(animacion);

            pajaralogin = FindViewById<ImageView>(Resource.Id.imageView4);
            pajaralogin.Click += (sender, e) =>
            {
                
                var builder = new AlertDialog.Builder(this);
                builder.SetTitle("Aviso");
                builder.SetMessage("Se cerrara la sesión ¿está seguro?");
                builder.SetPositiveButton("Aceptar", (sender2, args) => {


                    Toast.MakeText(this.ApplicationContext, "Adios", ToastLength.Long).Show();
                    //resultado.Text = respuesta.INSTANCIA.ID_INSTANCIA + " Operador: "+respuesta.id_Opr+" "+respuesta.nm_Oper;
                    ISharedPreferencesEditor editor = opciones.Edit();
                    editor.PutString("nm_opr", "");
                    editor.PutInt("id_opr", 0);
                    editor.PutString("nm_str", "");
                    editor.PutInt("instancia", 0);
                    editor.Commit();
                    StartActivity(typeof(MainActivity));

                });
                builder.SetNegativeButton("Cancelar", (sender2, args) => {
                     
                });
                builder.Show();

            };
        }

    }  
}