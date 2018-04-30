using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using SQLite;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace legalpro_cuestionarios
{
    [Activity(Label = "legalpro_cuestionarios inicio de session", MainLauncher = true, ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenSize | Android.Content.PM.ConfigChanges.Orientation, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : Activity
    {
        public static string Legalproprefs = "legalpropref";
        ISharedPreferences opciones;
        ImageView imagev;
        Android.Views.Animations.Animation animacion;
        sqlite_database_movements conexiondb;
        string folder;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            AppCenter.Start("aa8037b1-7b33-4856-ade1-94dc860bea06", typeof(Analytics), typeof(Crashes));
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            opciones = GetSharedPreferences(Legalproprefs, 0);
            if (opciones.GetString("nm_opr", "").Length > 0)
            {
                Intent intent = new Intent(this.ApplicationContext, typeof(Activity1));
                StartActivity(intent);
            }

            folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

            folder = System.IO.Path.Combine(folder, "legalprodb.db");
            ISharedPreferencesEditor editor = opciones.Edit();
            editor.PutString("db", folder);
            editor.Commit();

            conexiondb = new sqlite_database_movements();
            if (!System.IO.Directory.Exists(folder))
            {
                
                conexiondb.create_db(folder);
            }

            imagev = FindViewById<ImageView>(Resource.Id.nube);
            animacion = Android.Views.Animations.AnimationUtils.LoadAnimation(this, Resource.Animation.movimientolento5p);
            animacion.Reset();
            imagev.StartAnimation(animacion);

            imagev = FindViewById<ImageView>(Resource.Id.legalpro);
            animacion = Android.Views.Animations.AnimationUtils.LoadAnimation(this, Resource.Animation.movimientolento);
            animacion.Reset();
            imagev.StartAnimation(animacion);

            Button buton = FindViewById<Button>(Resource.Id.button);

            buton.Click += (sender, e) =>
            {
                
                 animacion= Android.Views.Animations.AnimationUtils.LoadAnimation(this, Resource.Animation.pushb);
                buton.StartAnimation(animacion);
                //string numerot = Core.logincode.ToNumber(phoneNumbertext.Text);
                //resultado.Text = numerot;
                new System.Threading.Thread(new System.Threading.ThreadStart(() => {
                    Login();
                })).Start();



            };

        }

        public void Login() {
            EditText usuario = FindViewById<EditText>(Resource.Id.usuario);
            EditText contraseña = FindViewById<EditText>(Resource.Id.password);
            

            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {

                WebReference1.logInIndata indata = new WebReference1.logInIndata();
                indata.nmopr = usuario.Text;
                indata.pwd = contraseña.Text;
                indata.urlExt = "";

                WebReference1.EJBWebServicev2_0 service = new WebReference1.EJBWebServicev2_0();

                WebReference1.logInResponse respuesta = service.LogInW(indata);
                if (respuesta.resultado > 0)
                {
                    //Toast.MakeText(this.ApplicationContext, "Bienvenido " + respuesta.nm_Oper , ToastLength.Long).Show();
                    //resultado.Text = respuesta.INSTANCIA.ID_INSTANCIA + " Operador: "+respuesta.id_Opr+" "+respuesta.nm_Oper;
                    ISharedPreferencesEditor editor = opciones.Edit();
                    editor.PutString("nm_opr", respuesta.nm_Oper);
                    editor.PutInt("id_opr", respuesta.id_Opr);
                    editor.PutString("nm_str", respuesta.tiendas[0].NM_STR_RT);
                    editor.PutInt("instancia", respuesta.id_instancia);
                    editor.Commit();

                    Usuarios data = new Usuarios();
                    data.ID = respuesta.id_Opr;
                    data.usuario = respuesta.nm_Oper;
                    data.instancia = respuesta.id_instancia;
                    data.password = contraseña.Text;
                    data.nm_str = respuesta.nm_Str_Rt;

                    string database = opciones.GetString("db", "");
                    var db = new SQLiteAsyncConnection(database);
                    var x = db.InsertAsync(data);

                    
                    Intent intent = new Intent(this.ApplicationContext, typeof(Activity1));
                    StartActivity(intent);

                }
                else { Toast.MakeText(this.ApplicationContext, "Error:" + respuesta.resultadoMsg, ToastLength.Long).Show(); }


            }
            else if (conexiondb.loginl(folder, usuario.Text, contraseña.Text, opciones))
            {
                Intent intent = new Intent(this.ApplicationContext, typeof(Activity1));
                StartActivity(intent);
                // return;
            }

            else {
                Toast.MakeText(this.ApplicationContext, "Usuario o password incorrecto", ToastLength.Long).Show();
            }


        }
 
    }
}

