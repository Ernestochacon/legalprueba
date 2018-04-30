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
    class upload
    {
        public static string Legalproprefs = "legalpropref";
        ISharedPreferences opciones;
        sqlite_database_movements db;
        WebReference1.formatos f;

        public upload(ISharedPreferences o)
        {
            opciones = o;
            db = new sqlite_database_movements();
        }

        public void subida() {
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                List<Guardados> por_subir = db.get_guardados_subir(opciones.GetString("db", ""));

                for (int x = 0; x < por_subir.Count; x++) {
                    if (por_subir[x].guardado == 0) { }
                    else if (por_subir[x].guardado == 1) {
                        f.COPIAS_TOTALES = 1;
                        f.FILE_NAME = por_subir[x].nombre;
                        f.ID_PROCESO = 1;
                        f.ID_VERSION = 1;
                        f.ID_CT = por_subir[x].usuario;
                        f.ID_USER = 0;
                        f.RESULTADO_XML = "";
                        f.ID_FORMATO = 0;
                    }
                }
            }
        }

    }
}