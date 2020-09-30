﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.Controls;
using DevComponents.DotNetBar.Rendering;

namespace EPME
{
    public partial class Factcli1 : DevComponents.DotNetBar.Office2007Form
    {
      
        private metier met = Program.met;
        DataSet ds;
        Boolean first = true, xvstk = false;
        decimal xdeb,xven;
        public Factcli1()
        {
            InitializeComponent();
        }


        private void Factcli1_Load(object sender, EventArgs e)
        {
            Program.desTfunction(this);

            buttonX1_Click(sender, e);
            if (Program.ISDemo)
            {
                if (dataGridViewX1.Rows.Count > 30)
                {
                    buttonX5.Enabled = false;
                }
            }
            first = false;
        }

       
        private void dataGridViewX1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            charger_lignefacture();
        }

       
        private void dataGridViewX1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            charger_lignefacture();
        }

        private void charger_lignefacture()
        {
            if(first)
                dataGridViewX1.Rows[dataGridViewX1.Rows.Count - 1].Selected = true;
            if (dataGridViewX1.SelectedRows.Count != 0)
            {
                // type Facture 
                if (dataGridViewX1.SelectedRows[0].Cells["mode"].Value.Equals("D")) 
                {
                    string sqlfac = "Select montreg From eentc where codes = '" + Program.Societe + "' AND codee ='" + Program.Exercice + "' AND codem ='" + Program.Magasin + "'  AND num = '" + dataGridViewX1.SelectedRows[0].Cells["num"].Value + "'";
                    DataSet tmpf = met.recuperer_table(sqlfac);
                    if (tmpf != null)
                        if (tmpf.Tables.Count != 0)
                        {
                            if (tmpf.Tables[0].Rows.Count != 0)
                            {
                                double xmntreg = 0;

                                double.TryParse(tmpf.Tables[0].Rows[0]["montreg"] + "", out xmntreg);
                                if (xmntreg == 0)
                                {
                                    buttonX2.Enabled = true;
                                    buttonX3.Enabled = true;
                                }
                                else
                                {
                                    buttonX2.Enabled = false;
                                    buttonX3.Enabled = false;
                                }
                            }
                        }
                }
                else
                {
                    string sqlfac = "Select montreg,mode From eentc where codes = '" + Program.Societe + "' AND codee ='" + Program.Exercice + "' AND codem ='" + Program.Magasin + "'  AND num = '" + dataGridViewX1.SelectedRows[0].Cells["num"].Value + "'";
                    DataSet tmpf = met.recuperer_table(sqlfac);
                    if (tmpf != null)
                        if (tmpf.Tables.Count != 0)
                        {
                            if (tmpf.Tables[0].Rows.Count != 0)
                            {
                                string xmode = tmpf.Tables[0].Rows[0]["mode"]+"";

                                double xmntreg = 0;

                                double.TryParse(tmpf.Tables[0].Rows[0]["montreg"] + "", out xmntreg);
                                if (xmntreg == 0)
                                {
                                    buttonX2.Enabled = false;
                                    buttonX3.Enabled = true;
                                }
                                else
                                {
                                    buttonX2.Enabled = false;

                                    if( xmode=="B")
                                       buttonX3.Enabled = true;
                                    else
                                        buttonX3.Enabled = false;
                                }
                            }
                        }
                    
                }

               

                // Chargement ligne Facture
                string req1 = "select distinct  l.num,l.codea, a.libelle , g.libelle as gamme , d.libelle as depot , l.qte , l.pu , l.tva ,l.taxe ,l.tvaf , l.rem , l.netht , l.netttc,l.codesa,l.ID   from lentc l , article a , gamme g , depot d where l.codes='" + Program.Societe + "' and l.codee='" + Program.Exercice + "' and l.num= '" + dataGridViewX1.SelectedRows[0].Cells["num"].Value + "' and  l.codem='" + Program.Magasin + "' and l.codea = a.code and l.codes= a.codes and l.codee= a.codee and l.codea = g.codea and l.codeg=g.codeg  and l.codes=g.codes and l.codee=g.codee and l.coded= d.code and l.codes=d.codes and l.codee=d.codee and l.codem=d.codem ";
                DataSet ds1 = met.recuperer_table(req1, "lentc");

                foreach (DataRow dr in ds1.Tables["lentc"].Rows)
                {

                    if (dr["codesa"] != DBNull.Value)
                    {
                        string sql = "select libelle from sarticle where codes='" + Program.Societe + "' and codee='" + Program.Exercice + "' and codea= '" + dr["codea"] + "' and code = '" + dr["codesa"] + "'";
                        DataSet dtmp = met.recuperer_table(sql, "sarticle");
                        dr["libelle"] = dtmp.Tables[0].Rows[0][0];
                        dr["codea"] = DBNull.Value;
                        dr["gamme"] = DBNull.Value;
                        dr["depot"] = DBNull.Value;
                    }

                }
                dataGridViewX2.DataSource = ds1.Tables["lentc"].DefaultView;

            }
        }


        private void buttonX3_Click(object sender, EventArgs e)
        {
            // Bouton Supprimer Facture
            if (dataGridViewX1.Rows.Count != 0)
            {
                if (dataGridViewX1.SelectedRows[0].Cells["mode"].Value+"" == "D")
                {
                    string sql = "select * from eentc where codes = '" + Program.Societe + "' and codee = '" + Program.Exercice + "'  and codem='" + Program.Magasin + "' and  num = '" + dataGridViewX1.SelectedRows[0].Cells["num"].Value + "' and regle = false";
                    DataSet dss = met.recuperer_table(sql, "eentc");
                    if (dss.Tables["eentc"].Rows.Count != 0)
                    {

                        String msg = MessageBox.Show("Ete-vous sur", "Important", MessageBoxButtons.YesNo, MessageBoxIcon.Question).ToString();
                        if (msg.Equals("Yes"))
                        {

                            string req1 = "Select * from lentc where codes='" + Program.Societe + "' and codee='" + Program.Exercice + "' and codem='" + Program.Magasin + "'  and  num = '" + dataGridViewX1.SelectedRows[0].Cells["num"].Value + "' ";
                            DataSet tmp1 = met.recuperer_table(req1, "lentc");
                            foreach (DataRow dr1 in tmp1.Tables["lentc"].Rows)
                            {
                                double xqte = 0, QTESTK = 0, QTESOR = 0, QTEMAG = 0, QTESORMAG = 0, QTESTE = 0, QTESORSTE = 0;
                                string xcodea = "";
                                xcodea = dr1.Field<string>("codea" + "");
                                string xcodeg = dr1.Field<string>("codeg" + "");
                                string xcoded = dr1.Field<string>("coded" + "");

                                xqte = dr1.Field<double>("qte");
                                try
                                {
                                    xqte = dr1.Field<double>("qte");
                                }
                                catch { }

                                #region // Update stock
                                string sql1 = "Select qtestk,qtesor from artdep where codes = '" + Program.Societe + "' and codee = '" + Program.Exercice + "' and codea ='" + xcodea + "' and coded='" + xcoded + "' and codem='" + Program.Magasin + "' and codeg='" + xcodeg + "'";
                                DataSet ds1 = met.recuperer_table(sql1, "artdep");
                                if (ds1 != null)
                                    if (ds1.Tables.Count != 0)
                                        if (ds1.Tables["artdep"].Rows.Count != 0)
                                        {
                                            Double.TryParse(ds1.Tables["artdep"].Rows[0]["qtestk"] + "", out QTESTK);
                                            Double.TryParse(ds1.Tables["artdep"].Rows[0]["qtesor"] + "", out QTESOR);
                                        }

                                string sql2 = "Select qtestk,qtesor from artmag where codes = '" + Program.Societe + "' and codee = '" + Program.Exercice + "' and codea ='" + xcodea + "'  and codem='" + Program.Magasin + "'";
                                DataSet ds2 = met.recuperer_table(sql2, "artmag");
                                if (ds2 != null)
                                    if (ds2.Tables.Count != 0)
                                        if (ds2.Tables["artmag"].Rows.Count != 0)
                                        {
                                            Double.TryParse(ds2.Tables["artmag"].Rows[0]["qtestk"] + "", out QTEMAG);
                                            Double.TryParse(ds2.Tables["artmag"].Rows[0]["qtesor"] + "", out QTESORMAG);
                                        }

                                string sql3 = "Select qtestk,qtesor,vstk from article where codes = '" + Program.Societe + "' and codee = '" + Program.Exercice + "' and code ='" + xcodea + "' ";
                                DataSet ds3 = met.recuperer_table(sql3, "article");
                                if (ds3 != null)
                                    if (ds3.Tables.Count != 0)
                                        if (ds3.Tables["article"].Rows.Count != 0)
                                        {
                                            xvstk = ds3.Tables["article"].Rows[0].Field<Boolean>("vstk");
                                            Double.TryParse(ds3.Tables["article"].Rows[0]["qtestk"] + "", out QTESTE);
                                            Double.TryParse(ds3.Tables["article"].Rows[0]["qtesor"] + "", out QTESORSTE);
                                        }
                              

                               
                                if (xvstk == true)
                                {
                                    string sqlup = "UPDATE artdep set qtestk = '" + (QTESTK+xqte) + "',qtesor='" + (QTESOR-xqte) + "' where codes = '" + Program.Societe + "' and codee = '" + Program.Exercice + "' and codea ='" + xcodea + "' and coded='" + xcoded + "' and codem='" + Program.Magasin + "' and codeg='" + xcodeg + "'";
                                    met.Execute(sqlup);

                                    sqlup = "UPDATE artmag set qtestk = '" + (QTEMAG+xqte) + "',qtesor='" + (QTESORMAG-xqte) + "' where codes = '" + Program.Societe + "' and codee = '" + Program.Exercice + "' and codea ='" + xcodea + "'  and codem='" + Program.Magasin + "'";
                                    met.Execute(sqlup);

                                    sqlup = "UPDATE article set qtestk = '" + (QTESTE+xqte) + "',qtesor='" + (QTESORSTE-xqte) + "' where codes = '" + Program.Societe + "' and codee = '" + Program.Exercice + "' and code ='" + xcodea + "' ";
                                    met.Execute(sqlup);

                                }
                                #endregion // Update stock
                            }


                            String req5 = "Select code,debit,solde From client where codes = '" + Program.Societe + "' AND codee ='" + Program.Exercice + "' AND Code = '" + dataGridViewX1.SelectedRows[0].Cells["codec"].Value + "'";
                            DataSet dsc = met.recuperer_table(req5, "client");
                            decimal wdeb = 0, wsld = 0;
                            try
                            {
                                wdeb = dsc.Tables["client"].Rows[0].Field<decimal>("debit");

                            }
                            catch { }

                            try
                            {
                                wsld = dsc.Tables["client"].Rows[0].Field<decimal>("solde");
                            }
                            catch { }

                            try
                            {
                                decimal.TryParse(dataGridViewX1.SelectedRows[0].Cells["TOTALTTC"].Value + "", out xdeb);
                            }
                            catch { }

                            String upcli = "UPDATE client SET debit = " + (wdeb - xdeb)
                                         + ", solde=" + (wsld - xdeb)
                                          + " where codes = '" + Program.Societe + "' AND codee ='" + Program.Exercice + "' AND Code = '" + dataGridViewX1.SelectedRows[0].Cells["codec"].Value + "'";
                            met.Execute(upcli);


                            String req6 = "Select code,vente,taux,com From rep where codes = '" + Program.Societe + "' AND codee ='" + Program.Exercice + "' AND Code = '" + dataGridViewX1.SelectedRows[0].Cells["coder"].Value + "'";
                            DataSet dsr = met.recuperer_table(req6, "rep");
                            decimal wven = 0, nven = 0;
                            double wcom = 0, wtaux = 0;
                            try
                            {
                                wven = dsr.Tables["rep"].Rows[0].Field<decimal>("vente");

                            }
                            catch { }
                            try
                            {
                                wtaux = dsr.Tables["rep"].Rows[0].Field<double>("taux");

                            }
                            catch { }

                            try
                            {
                                decimal.TryParse(dataGridViewX1.SelectedRows[0].Cells["TOTALHT"].Value + "", out xven);
                            }
                            catch { }

                            nven = wven - xven;
                            wcom = (double)nven * (wtaux / 100);
                            String uprep = "UPDATE rep SET vente = " + nven
                                         + ", com=" + wcom
                                          + " where codes = '" + Program.Societe + "' AND codee ='" + Program.Exercice + "' AND Code = '" + dataGridViewX1.SelectedRows[0].Cells["coder"].Value + "'";
                            met.Execute(uprep);



                            String req3 = "DELETE FROM eentc Where codes = '" + Program.Societe + "' and codee = '" + Program.Exercice + "' and codem='" + Program.Magasin + "' and num = '" + dataGridViewX1.SelectedRows[0].Cells["num"].Value + "'";

                            if (met.Execute(req3))
                            {
                                String req4 = "DELETE FROM lentc Where codes = '" + Program.Societe + "' and codee = '" + Program.Exercice + "' and codem='" + Program.Magasin + "' and num = '" + dataGridViewX1.SelectedRows[0].Cells["num"].Value + "'";
                                if (met.Execute(req4))
                                    MessageBox.Show("Suppression effectuée");
                            }
                            buttonX1_Click(sender, e);
                        }

                    }
                    else
                    {
                        MessageBox.Show("Facture Reglée");
                    }
                }
                else if (dataGridViewX1.SelectedRows[0].Cells["mode"].Value+"" == "L")
                {
                    string sql = "select * from eentc where codes = '" + Program.Societe + "' and codee = '" + Program.Exercice + "'  and codem='" + Program.Magasin + "' and  num = '" + dataGridViewX1.SelectedRows[0].Cells["num"].Value + "' and regle = false";
                    DataSet dss = met.recuperer_table(sql, "eentc");
                    if (dss.Tables["eentc"].Rows.Count != 0)
                    {
                          String msg = MessageBox.Show("Ete-vous sur", "Important", MessageBoxButtons.YesNo, MessageBoxIcon.Question).ToString();
                          if (msg.Equals("Yes"))
                          {
                              string sqln = "select * from nblfacture where codes = '" + Program.Societe + "' and codee = '" + Program.Exercice + "'  and codem='" + Program.Magasin + "' and  numf = '" + dataGridViewX1.SelectedRows[0].Cells["num"].Value + "'";
                              DataSet dsn = met.recuperer_table(sqln, "nblfacture");
                              if (dsn != null)
                              {
                                  foreach (DataRow dr in dsn.Tables["nblfacture"].Rows)
                                  {
                                      string xnbl = dr.Field<string>("numl");

                                      String upbl = "UPDATE eentl SET facture = " + false
                                                  + " where codes = '" + Program.Societe + "' AND codee ='" + Program.Exercice + "' AND codem ='" + Program.Magasin + "' AND num = '" + xnbl + "'";
                                      met.Execute(upbl);

                                  }
                              }

                              String req3 = "DELETE FROM eentc Where codes = '" + Program.Societe + "' and codee = '" + Program.Exercice + "' and codem='" + Program.Magasin + "' and num = '" + dataGridViewX1.SelectedRows[0].Cells["num"].Value + "'";
                              met.Execute(req3);
                              String req4 = "DELETE FROM lentc Where codes = '" + Program.Societe + "' and codee = '" + Program.Exercice + "' and codem='" + Program.Magasin + "' and num = '" + dataGridViewX1.SelectedRows[0].Cells["num"].Value + "'";
                              met.Execute(req4);
                              String req5 = "DELETE FROM nblfacture Where codes = '" + Program.Societe + "' and codee = '" + Program.Exercice + "' and codem='" + Program.Magasin + "' and numf = '" + dataGridViewX1.SelectedRows[0].Cells["num"].Value + "'";
                              met.Execute(req5);
                              MessageBox.Show("Suppression effectuée");
                              buttonX1_Click(sender, e);
                          }
                    }
                    else
                    {
                        MessageBox.Show("Facture Reglée");
                    }

                }
                else if (dataGridViewX1.SelectedRows[0].Cells["mode"].Value+"" == "B")
                {
                    string sql = "select * from eentc where codes = '" + Program.Societe + "' and codee = '" + Program.Exercice + "'  and codem='" + Program.Magasin + "' and  num = '" + dataGridViewX1.SelectedRows[0].Cells["num"].Value + "'";
                    DataSet dss = met.recuperer_table(sql, "eentc");
                    if (dss.Tables["eentc"].Rows.Count != 0)
                    {
                        String msg = MessageBox.Show("Ete-vous sur", "Important", MessageBoxButtons.YesNo, MessageBoxIcon.Question).ToString();
                        if (msg.Equals("Yes"))
                        {
                            string sqln = "select * from nblfacture where codes = '" + Program.Societe + "' and codee = '" + Program.Exercice + "'  and codem='" + Program.Magasin + "' and  numf = '" + dataGridViewX1.SelectedRows[0].Cells["num"].Value + "'";
                            DataSet dsn = met.recuperer_table(sqln, "nblfacture");
                            if (dsn != null)
                            {
                                foreach (DataRow dr in dsn.Tables["nblfacture"].Rows)
                                {
                                    string xnbl = dr.Field<string>("numl");

                                    String upbl = "UPDATE eentcb SET facture = " + false
                                                + " where codes = '" + Program.Societe + "' AND codee ='" + Program.Exercice + "' AND codem ='" + Program.Magasin + "' AND num = '" + xnbl + "'";
                                    met.Execute(upbl);

                                }
                            }

                            String req3 = "DELETE FROM eentc Where codes = '" + Program.Societe + "' and codee = '" + Program.Exercice + "' and codem='" + Program.Magasin + "' and num = '" + dataGridViewX1.SelectedRows[0].Cells["num"].Value + "'";
                            met.Execute(req3);
                            String req4 = "DELETE FROM lentc Where codes = '" + Program.Societe + "' and codee = '" + Program.Exercice + "' and codem='" + Program.Magasin + "' and num = '" + dataGridViewX1.SelectedRows[0].Cells["num"].Value + "'";
                            met.Execute(req4);
                            String req5 = "DELETE FROM nblfacture Where codes = '" + Program.Societe + "' and codee = '" + Program.Exercice + "' and codem='" + Program.Magasin + "' and numf = '" + dataGridViewX1.SelectedRows[0].Cells["num"].Value + "'";
                            met.Execute(req5);
                            MessageBox.Show("Suppression effectuée");
                            buttonX1_Click(sender, e);
                        }
                    }
                    
                }
            }
        }

        private void buttonItem5_Click(object sender, EventArgs e)
        {
            {
               

            }
        }

        private void buttonItem6_Click(object sender, EventArgs e)
        {
            // Bouton créer Facture BL

           
            buttonX1_Click(sender, e);
        }
        private void buttonItem7_Click(object sender, EventArgs e)
        {
            // Bouton créer Facture Co

            
        }

        private void buttonX4_Click(object sender, EventArgs e)
        {

            // Bouton Recherche
            if (dataGridViewX1.Rows.Count != 0)
            {
                Rechfact form = new Rechfact();
                form.table = "Client";
                form.ShowDialog();
                if (!form.textBoxX3.Text.Equals(""))
                {
                    DataRow[] dr = ds.Tables["eentc"].Select("num = '" + form.textBoxX3.Text + "'");
                    if (dr.Length != 0)
                    {
                        int i = ds.Tables[0].Rows.IndexOf(dr[0]);
                        dataGridViewX1.Rows[i].Selected = true;
                    }
                    else
                    {
                        MessageBox.Show("N° Facture n'existe pas.");
                    }
                }
                else if (!form.code.Equals("0") && !form.dateTimeInput1.Text.Equals("") && !form.dateTimeInput2.Text.Equals(""))
                {

                    string ddebut = form.dateTimeInput1.Value.ToString("yyyy-MM-dd");
                    string dfin = form.dateTimeInput2.Value.ToString("yyyy-MM-dd");
                    string req2 = "select num,date,codec,nomc,totalht,fodec,totalrem,totaltva,totaltaxe,timbre,totalttc,montavr,montreg,reste,mode,coder from eentc where codes = '" + Program.Societe + "' and codee = '" + Program.Exercice + "' AND codem ='" + Program.Magasin + "' and (codec='" + form.code + "' AND date>='" + ddebut + "' AND date<='" + dfin + "') order by num";
                    ds = met.recuperer_table(req2, "eentc");
                    dataGridViewX1.DataSource = ds.Tables["eentc"].DefaultView;
                    if (dataGridViewX1.Rows.Count != 0)
                    {
                        charger_lignefacture();
                        total();
                    }
                    buttonX1.Visible = true;
                }
                else if (!form.code.Equals("0") && !form.dateTimeInput1.Text.Equals(""))
                {


                    string ddebut = form.dateTimeInput1.Value.ToString("yyyy-MM-dd");
                    string dfin = form.dateTimeInput2.Value.ToString("yyyy-MM-dd");
                    string req2 = "select num,date,codec,nomc,totalht,fodec,totalrem,totaltva,totaltaxe,timbre,totalttc,montavr,montreg,reste,mode,coder from eentc where codes = '" + Program.Societe + "' and codee = '" + Program.Exercice + "' AND codem ='" + Program.Magasin + "' and (codec='" + form.code + "' AND date>='" + ddebut + "' ) order by num";
                    ds = met.recuperer_table(req2, "eentc");
                    dataGridViewX1.DataSource = ds.Tables["eentc"].DefaultView;
                    if (dataGridViewX1.Rows.Count != 0)
                    {
                        charger_lignefacture();
                        total();
                    }
                    buttonX1.Visible = true;
                }
                else if (!form.code.Equals("0"))
                {


                    string ddebut = form.dateTimeInput1.Value.ToString("yyyy-MM-dd");
                    string dfin = form.dateTimeInput2.Value.ToString("yyyy-MM-dd");
                    string req2 = "select num,date,codec,nomc,totalht,fodec,totalrem,totaltva,totaltaxe,timbre,totalttc,montavr,montreg,reste,mode,coder from eentc where codes = '" + Program.Societe + "' and codee = '" + Program.Exercice + "' AND codem ='" + Program.Magasin + "' and  (codec='" + form.code + "') order by num";
                    ds = met.recuperer_table(req2, "eentc");
                    dataGridViewX1.DataSource = ds.Tables["eentc"].DefaultView;
                    if (dataGridViewX1.Rows.Count != 0)
                    {
                        charger_lignefacture();
                        total();
                    }
                    buttonX1.Visible = true;
                }
                else if (form.code.Equals("0") && !form.dateTimeInput1.Text.Equals("") && !form.dateTimeInput2.Text.Equals(""))
                {


                    string ddebut = form.dateTimeInput1.Value.ToString("yyyy-MM-dd");
                    string dfin = form.dateTimeInput2.Value.ToString("yyyy-MM-dd");
                    string req2 = "select num,date,codec,nomc,totalht,fodec,totalrem,totaltva,totaltaxe,timbre,totalttc,montavr,montreg,reste,mode,coder from eentc where codes = '" + Program.Societe + "' and codee = '" + Program.Exercice + "' AND codem ='" + Program.Magasin + "' and (date>='" + ddebut + "' AND date<='" + dfin + "') order by num";
                    ds = met.recuperer_table(req2, "eentc");
                    dataGridViewX1.DataSource = ds.Tables["eentc"].DefaultView;
                    if (dataGridViewX1.Rows.Count != 0)
                    {
                        charger_lignefacture();
                        total();
                    }
                    buttonX1.Visible = true;
                }
                else if (form.code.Equals("0") && !form.dateTimeInput1.Text.Equals(""))
                {


                    string ddebut = form.dateTimeInput1.Value.ToString("yyyy-MM-dd");
                    string dfin = form.dateTimeInput2.Value.ToString("yyyy-MM-dd");
                    string req2 = "select num,date,codec,nomc,totalht,fodec,totalrem,totaltva,totaltaxe,timbre,totalttc,montavr,montreg,reste,mode,coder from eentc where codes = '" + Program.Societe + "' and codee = '" + Program.Exercice + "' AND codem ='" + Program.Magasin + "' and (date>='" + ddebut + "' ) order by num";
                    ds = met.recuperer_table(req2, "eentc");
                    dataGridViewX1.DataSource = ds.Tables["eentc"].DefaultView;
                    if (dataGridViewX1.Rows.Count != 0)
                    {
                        charger_lignefacture();
                        total();
                    }
                    buttonX1.Visible = true;
                }
                else
                    buttonX1_Click(sender, e);
            }
          
        }

        private void Factcli1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {

                case Keys.F1:
                    {
                        buttonX4_Click(sender, e);
                    }
                    break;

                case Keys.F3:
                    {
                        superTabControl1.SelectedTab = tab1;
                    }
                    break;

                case Keys.F4:
                    {
                        superTabControl1.SelectedTab = tab2;
                    }
                    break;

                default:
                    // actions_sinon;
                    break;
            }
        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            buttonX1.Visible = false;


            string req = "select num,date,codec,nomc,totalht,fodec,totalrem,totaltva,totaltaxe,timbre,totalttc,montavr,montreg,reste,mode,coder from eentc where codes = '" + Program.Societe + "' and codee = '" + Program.Exercice + "' AND codem ='" + Program.Magasin + "' order by num";
            ds = met.recuperer_table(req, "eentc");
            dataGridViewX1.DataSource = ds.Tables["eentc"].DefaultView;
            if (dataGridViewX1.Rows.Count != 0)
            {
                if (first)
                {
                    dataGridViewX1.CurrentCell = dataGridViewX1.Rows[dataGridViewX1.Rows.Count - 1].Cells[0];
                }
                dataGridViewX1.Rows[0].Selected = false;
                dataGridViewX1.FirstDisplayedScrollingRowIndex = dataGridViewX1.Rows.Count - 1;
                dataGridViewX1.Rows[dataGridViewX1.Rows.Count - 1].Selected = true;
                charger_lignefacture();
                total();
            }
        }

        private void total()
        {
            Decimal xth = 0 , xtrem = 0, xtva = 0, xttc = 0 , xavr = 0 , xtreg = 0 ,xttaxe=0, xtsld = 0 ,xfodec=0;
            Decimal x1 = 0, x2 = 0 ,x3 = 0;
            foreach (DataRow dr in ds.Tables["eentc"].Rows)
            {

                string xf = dr.Field<string>("mode");

                try
                {
                    xth += dr.Field<Decimal>("totalht");
                }
                catch { }

                try
                {
                    xfodec += dr.Field<Decimal>("fodec");
                }
                catch { }

                try
                {
                    xtrem += dr.Field<Decimal>("totalrem");
                }
                catch { }

                try
                {
                    xtva += dr.Field<Decimal>("totaltva");
                }
                catch { }
                try
                {
                    xttc += dr.Field<Decimal>("totalttc");
                }
                catch { }
                try
                {
                    xtreg += dr.Field<Decimal>("montreg");
                }
                catch { }
                try
                {
                    xtsld += dr.Field<Decimal>("reste");
                }
                catch { }
                try
                {
                    xttaxe += dr.Field<Decimal>("totaltaxe");
                }
                catch { }

                try
                {
                    xavr += dr.Field<Decimal>("montavr");
                }
                catch { }

                if (xf=="D")
                {
                    try
                    {
                        x1 += dr.Field<Decimal>("totalttc");
                    }
                    catch { }
                }
                else if (xf=="L")
                {
                    try
                    {
                        x2 += dr.Field<Decimal>("totalttc");
                    }
                    catch { }
                }
                else if (xf == "B")
                {
                    try
                    {
                        x3 += dr.Field<Decimal>("totalttc");
                    }
                    catch { }
                }

                textBoxX1.Text = xth.ToString("N3");
                textBoxX2.Text = xtrem.ToString("N3");
                textBoxX3.Text = xtva.ToString("N3");
                textBoxX4.Text = xttc.ToString("N3");
                textBoxX5.Text = xtreg.ToString("N3");
                textBoxX6.Text = xtsld.ToString("N3");
                textBoxX7.Text = xavr.ToString("N3");
                textBoxX8.Text = xfodec.ToString("N3");
                textBoxX15.Text = xttaxe.ToString("N3");

                textBoxX10.Text = x1.ToString("N3");
                textBoxX9.Text = x2.ToString("N3");
                textBoxX14.Text = x3.ToString("N3");
            }
        }

        private void dataGridViewX2_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void buttonX2_Click(object sender, EventArgs e)
        {
            if (dataGridViewX1.Rows.Count != 0)
            {
               
            }
        }


        private void dataGridViewX1_DoubleClick(object sender, EventArgs e)
        {
            superTabControl1.SelectedTab = tab2;
        }

        private void dataGridViewX2_DoubleClick(object sender, EventArgs e)
        {
            superTabControl1.SelectedTab = tab1;
        }

        private void superTabControlPanel2_Click(object sender, EventArgs e)
        {

        }

        private void Factcli1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Program.desfunctionf(this);
        }

        private void dataGridViewX1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            foreach (DataGridViewRow dgr in dataGridViewX1.Rows)
            {


                string xtype = dgr.Cells["mode"].Value + "";

                if (xtype == "D")
                {
                    dgr.Cells["num"].Style.ForeColor = Color.Black;
                    dgr.Cells["num"].Style.BackColor = Color.White;
                    dgr.Cells["date"].Style.ForeColor = Color.Black;
                    dgr.Cells["date"].Style.BackColor = Color.White;
                    dgr.Cells["codec"].Style.ForeColor = Color.Black;
                    dgr.Cells["codec"].Style.BackColor = Color.White;
                    dgr.Cells["libellec"].Style.ForeColor = Color.Black;
                    dgr.Cells["libellec"].Style.BackColor = Color.White;

                    dgr.Cells["totalht"].Style.ForeColor = Color.Black;
                    dgr.Cells["totalht"].Style.BackColor = Color.White;
                    dgr.Cells["fodec"].Style.ForeColor = Color.Black;
                    dgr.Cells["fodec"].Style.BackColor = Color.White;
                    dgr.Cells["totaltva"].Style.ForeColor = Color.Black;
                    dgr.Cells["totaltva"].Style.BackColor = Color.White;
                    dgr.Cells["totalrem"].Style.ForeColor = Color.Black;
                    dgr.Cells["totalrem"].Style.BackColor = Color.White;
                    dgr.Cells["totalttc"].Style.ForeColor = Color.Black;
                    dgr.Cells["totalttc"].Style.BackColor = Color.White;
                    dgr.Cells["timbre"].Style.ForeColor = Color.Black;
                    dgr.Cells["timbre"].Style.BackColor = Color.White;
                    dgr.Cells["totaltaxe"].Style.ForeColor = Color.Black;
                    dgr.Cells["totaltaxe"].Style.BackColor = Color.White;
                    dgr.Cells["montavr"].Style.ForeColor = Color.Black;
                    dgr.Cells["montavr"].Style.BackColor = Color.White;
                    dgr.Cells["montreg"].Style.ForeColor = Color.Black;
                    dgr.Cells["montreg"].Style.BackColor = Color.White;
                    dgr.Cells["reste"].Style.ForeColor = Color.Black;
                    dgr.Cells["reste"].Style.BackColor = Color.White;
                }
                
                else if (xtype == "L")
                {

                    dgr.Cells["num"].Style.ForeColor = Color.Black;
                    dgr.Cells["num"].Style.BackColor = Color.Yellow;
                    dgr.Cells["date"].Style.ForeColor = Color.Black;
                    dgr.Cells["date"].Style.BackColor = Color.Yellow;
                    dgr.Cells["codec"].Style.ForeColor = Color.Black;
                    dgr.Cells["codec"].Style.BackColor = Color.Yellow;
                    dgr.Cells["libellec"].Style.ForeColor = Color.Black;
                    dgr.Cells["libellec"].Style.BackColor = Color.Yellow;

                    dgr.Cells["totalht"].Style.ForeColor = Color.Black;
                    dgr.Cells["totalht"].Style.BackColor = Color.Yellow;
                    dgr.Cells["fodec"].Style.ForeColor = Color.Black;
                    dgr.Cells["fodec"].Style.BackColor = Color.Yellow;
                    dgr.Cells["totaltva"].Style.ForeColor = Color.Black;
                    dgr.Cells["totaltva"].Style.BackColor = Color.Yellow;
                    dgr.Cells["totalrem"].Style.ForeColor = Color.Black;
                    dgr.Cells["totalrem"].Style.BackColor = Color.Yellow;
                    dgr.Cells["totalttc"].Style.ForeColor = Color.Black;
                    dgr.Cells["totalttc"].Style.BackColor = Color.Yellow;
                    dgr.Cells["timbre"].Style.ForeColor = Color.Black;
                    dgr.Cells["timbre"].Style.BackColor = Color.Yellow;
                    dgr.Cells["totaltaxe"].Style.ForeColor = Color.Black;
                    dgr.Cells["totaltaxe"].Style.BackColor = Color.Yellow;
                    dgr.Cells["montavr"].Style.ForeColor = Color.Black;
                    dgr.Cells["montavr"].Style.BackColor = Color.Yellow;
                    dgr.Cells["montreg"].Style.ForeColor = Color.Black;
                    dgr.Cells["montreg"].Style.BackColor = Color.Yellow;
                    dgr.Cells["reste"].Style.ForeColor = Color.Black;
                    dgr.Cells["reste"].Style.BackColor = Color.Yellow;
                    
                }
                else if (xtype == "B")
                {

                    dgr.Cells["num"].Style.ForeColor = Color.White;
                    dgr.Cells["num"].Style.BackColor = Color.Red;
                    dgr.Cells["date"].Style.ForeColor = Color.White;
                    dgr.Cells["date"].Style.BackColor = Color.Red;
                    dgr.Cells["codec"].Style.ForeColor = Color.White;
                    dgr.Cells["codec"].Style.BackColor = Color.Red;
                    dgr.Cells["libellec"].Style.ForeColor = Color.White;
                    dgr.Cells["libellec"].Style.BackColor = Color.Red;

                    dgr.Cells["totalht"].Style.ForeColor = Color.White;
                    dgr.Cells["totalht"].Style.BackColor = Color.Red;
                    dgr.Cells["fodec"].Style.ForeColor = Color.White;
                    dgr.Cells["fodec"].Style.BackColor = Color.Red;
                    dgr.Cells["totaltva"].Style.ForeColor = Color.White;
                    dgr.Cells["totaltva"].Style.BackColor = Color.Red;
                    dgr.Cells["totalrem"].Style.ForeColor = Color.White;
                    dgr.Cells["totalrem"].Style.BackColor = Color.Red;
                    dgr.Cells["totalttc"].Style.ForeColor = Color.White;
                    dgr.Cells["totalttc"].Style.BackColor = Color.Red;
                    dgr.Cells["timbre"].Style.ForeColor = Color.White;
                    dgr.Cells["timbre"].Style.BackColor = Color.Red;
                    dgr.Cells["totaltaxe"].Style.ForeColor = Color.White;
                    dgr.Cells["totaltaxe"].Style.BackColor = Color.Red;
                    dgr.Cells["montavr"].Style.ForeColor = Color.White;
                    dgr.Cells["montavr"].Style.BackColor = Color.Red;
                    dgr.Cells["montreg"].Style.ForeColor = Color.White;
                    dgr.Cells["montreg"].Style.BackColor = Color.Red;
                    dgr.Cells["reste"].Style.ForeColor = Color.White;
                    dgr.Cells["reste"].Style.BackColor = Color.Red;


                }
            }
        }

       

        private void buttonItem8_Click(object sender, EventArgs e)
        {
            if (dataGridViewX1.Rows.Count != 0)
            {
                if (dataGridViewX1.SelectedRows[0].Cells["mode"].Value.Equals("D"))
                {
                }
                else if (dataGridViewX1.SelectedRows[0].Cells["mode"].Value.Equals("L"))
                {
                   
                }
                else if (dataGridViewX1.SelectedRows[0].Cells["mode"].Value.Equals("B"))
                {
                   
                }
            }
        }

        private void buttonItem9_Click(object sender, EventArgs e)
        {
            if (dataGridViewX1.Rows.Count != 0)
            {
               
            }
        }

    }
}