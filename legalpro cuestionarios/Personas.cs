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
    class Personas
    {
        [PrimaryKey]
        public int ID { get; set; }

        public string FN_PRS { get; set; }//nombre

        public string LN_PRS { get; set; }//apellido

        public string NM_PRS_SFX { get; set; }//materno

        public string EDO_CIVIL { get; set; }

        public string LU_NAC { get; set; } //lugar luvnacimiento

        public string DC_PRS_BRT { get; set; }//fecha de nacimiento

        public string OCUPACION { get; set; }

        public string DOMICILIO { get; set; }

        public string NO_EXT { get; set; }

        public string NO_INT { get; set; }

        public string COLONIA { get; set; }

        public string CIUDAD { get; set; }

        public string MUNICIPIO { get; set; }

        public string ESTADO { get; set; }

        public string PAIS { get; set; }

        public string TEL_TA_PH { get; set; }//telefono

        public string CEL_TA_PH { get; set; }//celular

        public string EM_ADS { get; set; }//correo

        public string TIPO_IDENTIF { get; set; }//tipo identificacion

        public string FOLIO_IDENTIF { get; set; }//folio identificacion

        public string CURP { get; set; }


        public override string ToString()
        {
            return string.Format("[Personas: ID={0}, FN_PRS={1}, LN_PRS={2}, NM_PRS_SFX={3},CURP={4}]", ID, FN_PRS, LN_PRS, NM_PRS_SFX, CURP);
        }
    }
}