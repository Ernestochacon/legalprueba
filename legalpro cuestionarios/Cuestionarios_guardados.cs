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
    [Activity(Label = "Cuestionarios guardados", ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenSize | Android.Content.PM.ConfigChanges.Orientation, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class Cuestionarios_guardados : Activity
    {
        List<Listageneral> general = new List<Listageneral>();
        public static string Legalproprefs = "legalpropref";
        ISharedPreferences opciones;
        ListView lista_modelos;
        sqlite_database_movements db;
        bool click; 
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Lista_guardados);
            // Create your application here
            db = new sqlite_database_movements();
            click = true;
            opciones = GetSharedPreferences(Legalproprefs, 0);

            if (opciones.GetString("nm_opr", "").Length == 0)
            {
                Intent intent = new Intent(this.ApplicationContext, typeof(MainActivity));
                StartActivity(intent);
            }
            RunOnUiThread(() => {
                
                get_cuestionarios();
                
            });

            FindViewById<ImageView>(Resource.Id.imageView4).Click += (sender, e) =>
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

        public void get_cuestionarios() {
            general = db.guardados_local(opciones.GetString("db", ""), opciones.GetInt("instancia", 0),opciones.GetInt("id_opr",0));
            List_the_list();
        }

        void List_the_list()
        {
            /*
            var builder = new AlertDialog.Builder(this);
            builder.SetTitle("Aviso");
            builder.SetMessage("Los Cuestionarios verdes ya estan descargados");
            builder.SetPositiveButton("ok", (sender2, args) => { });
            builder.Show();*/

            lista_modelos = FindViewById<ListView>(Resource.Id.listam);
            lista_modelos.Adapter = new Adapterlist(this, general);
            //Toast.MakeText(this.ApplicationContext, error, ToastLength.Long).Show();
            lista_modelos.ItemLongClick += new EventHandler<AdapterView.ItemLongClickEventArgs>(ItemLong_OnClick);
        }

        private void ItemLong_OnClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            int index = e.Position;
            Intent intent = new Intent(this.ApplicationContext, typeof(EditarCuestionario));
            string[] valores = general[index].descripcion.Split(',');
            intent.PutExtra("modelo", valores[1]);
            intent.PutExtra("id_modelo", valores[0]);
            intent.PutExtra("id_formato", general[index].formato);
            StartActivity(intent);
        }

        }
}