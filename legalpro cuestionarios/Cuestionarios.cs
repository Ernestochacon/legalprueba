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
    class Cuestionarios
    {
        [PrimaryKey]
        public int id_cuestionario { get; set; }

        public string nombre { get; set; }

        public string xml { get; set; }

        public int instancia { get; set; }

        public string anexos { get; set; }

        public override string ToString()
        {
            return string.Format("[Cuestionarios: id_cuestionario={0}, nombre={1}, xml={2},instancia={3}]", id_cuestionario, nombre, xml,instancia);
        }

    }
}