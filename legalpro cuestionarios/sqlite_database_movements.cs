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
    class sqlite_database_movements
    {
        ISharedPreferences opciones;
        public static string Legalproprefs = "legalpropref";


        public void create_db(string path)
        {
            var conn = new SQLiteConnection(path);
            conn.CreateTable<Cuestionarios>();
            conn.CreateTable<Usuarios>();
            conn.CreateTable<Guardados>();
            conn.CreateTable<Personas>();
            conn.Dispose();
        }

        public Boolean loginl(string path, string u, string p,ISharedPreferences opc) {
            opciones = opc;
            var conn = new SQLiteConnection(path);
            //var asa = conn.QueryAsync<Usuarios>("SELECT * FROM Usuarios where usuario='"+u+ "' password='"+p+"'");
            var asa = conn.Table<Usuarios>();
            foreach (var s in asa)
            {
                if (s.usuario.Equals(u) && s.password.Equals(p)) {
                    ISharedPreferencesEditor editor = opciones.Edit();
                    editor.PutString("nm_opr", s.usuario);
                    editor.PutInt("id_opr", s.ID);
                    editor.PutString("nm_str", s.nm_str);
                    editor.PutInt("instancia", s.instancia);
                    editor.PutString("db", path);
                    editor.Commit();
                    return true;
                }
            }
            return false;
        }

        public List<Listageneral> cuestionarioslocal(string path, int instancia) {
            var conn = new SQLiteConnection(path);
            var asa = conn.Table<Cuestionarios>();


            List<Listageneral> general = new List<Listageneral>();
            Listageneral temp;
            foreach (var s in asa) {
                if (s.instancia == instancia) {
                    temp = new Listageneral(s.id_cuestionario, s.nombre, "°", 2);
                    general.Add(temp);
                }
            }
            return general;
        }

        public List<Listageneral> guardardos(string path,int usuario) {
            var conn = new SQLiteConnection(path);
            var asa = conn.Table<Guardados>();
            List<Listageneral> general = new List<Listageneral>();
            Listageneral temp;
            foreach (var s in asa)
            {
                if (s.usuario == usuario)
                {
                    temp = new Listageneral(s.id_formato, s.nombre, "°", 1);
                    general.Add(temp);
                }
            }
            return general;
        }

        public List<int> listam(string path, int instancia){
            var conn = new SQLiteConnection(path);
            var asa = conn.Table<Cuestionarios>();
            List<int> modelos = new List<int>();

            foreach (var s in asa)
            {
                if (s.instancia == instancia)
                {
                    modelos.Add(s.id_cuestionario);
                }
            }
            return modelos;
        }

        public Cuestionarios get_cuestionarioS(int id_modelo,int id_instancia,String path) {
            var conn = new SQLiteConnection(path);
            var asa = conn.Table<Cuestionarios>();
            Cuestionarios regreso= new Cuestionarios();

            foreach (var s in asa)
            {
                if (s.instancia == id_instancia&&s.id_cuestionario==id_modelo)
                {
                    regreso = s;
                }
            }
            return regreso;
        }


        public Guardados get_guardado(int id_formato, int id_instancia, string path)
        {
            var conn = new SQLiteConnection(path);
            var asa = conn.Table<Guardados>();
            Guardados regreso = new Guardados();

            foreach (var s in asa)
            {
                if (s.id_instancia == id_instancia && s.id_formato == id_formato)
                {
                    regreso = s;
                }
            }
            return regreso;
        }

        public int update_guardado(Guardados a, String path) {
            var conn = new SQLiteConnection(path);
            if (conn.Update(a) > 0)
            {
                return 1;
            }
            else return 0;
        }

        public int insert_guardado(Guardados a, String path)
        {
            var conn = new SQLiteConnection(path);
            if (conn.Insert(a) > 0)
            {
                return 1;
            }
            else return 0;
        }

        public List<Guardados> get_guardados(int id_usuario,int id_instancia,string path) {
            var conn = new SQLiteConnection(path);
            var asa = conn.Table<Guardados>();
            List<Guardados> regreso = new List<Guardados>();

            foreach (var s in asa) {
                if (s.id_instancia == id_instancia && s.usuario == id_usuario)
                {
                    regreso.Add(s);
                }
            }
            return regreso;
        }//obtener todos los guardados por instancia, usuario

        public List<Guardados> get_guardados_subir(string path)
        {
            var conn = new SQLiteConnection(path);
            var asa = conn.Table<Guardados>();
            List<Guardados> regreso = new List<Guardados>();

            foreach (var s in asa)
            {
                if (s.guardado!= 2)
                {
                    regreso.Add(s);
                }
            }
            return regreso;
        }//obtener todos los guardados por instancia, usuario

        public List<Listageneral> guardados_local(string path, int instancia, int id_usuario)
        {
            var conn = new SQLiteConnection(path);
            var asa = conn.Table<Guardados>();


            List<Listageneral> general = new List<Listageneral>();
            Listageneral temp;
            foreach (var s in asa)
            {
                if (s.id_instancia == instancia &&s.usuario==id_usuario)
                {
                    temp = new Listageneral(s.id_formato, s.nombre,s.id_modelo+","+ s.nm_modelo, 2);
                    general.Add(temp);
                }
            }
            return general;
        }
    }
}