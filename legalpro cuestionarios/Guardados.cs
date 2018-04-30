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
using SQLite;

namespace legalpro_cuestionarios
{
    class Guardados
    {
        [PrimaryKey,AutoIncrement]
        public int id_formato { get; set; }

        public int id_instancia { get; set; }

        public string nombre { get; set; }

        public string xml { get; set; }

        public int guardado { get; set; }

        public int usuario { get; set; }

        public int id_modelo { get; set; }

        public string nm_modelo { get; set; }

        public string container { get; set; }

        public string containe2r { get; set; }

        public string preguntas { get; set; }

        

        public override string ToString()
        {
            return string.Format("[Guardados: id_formato={0}, nombre={1}, xml={2}, guardado={3},usuario={4}]", id_formato, nombre, xml, guardado,usuario);
        }

    }
}