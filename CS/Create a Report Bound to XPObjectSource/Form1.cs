﻿using Create_a_Report_Bound_to_XPObjectSource.nwind;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Create_a_Report_Bound_to_XPObjectSource {
    public partial class Form1 : Form {

        // A data layer instance that is used in the ResolveSession/DismissSession event handlers
        IDataLayer DataLayerInstance = null;

        public Form1() {
            InitializeComponent();
        }

        private XPObjectSource CreateDataSource() {
            ReflectionDictionary dictionary = new ReflectionDictionary();
            XPClassInfo classInfo = dictionary.QueryClassInfo(typeof(Products));
            XPObjectSource dataSource = new XPObjectSource() {
                ConnectionStringName = "nwind",
                EntityType = classInfo
            };

            return dataSource;
        }

        private XtraReport CreateReport() {
            XtraReport report = new XtraReport {
                Bands = {
                    new DetailBand {
                        Controls = {
                            new XRLabel {
                                ExpressionBindings = {
                                    new ExpressionBinding("BeforePrint", "Text", "[ProductName]")
                                },
                                WidthF = 300
                            }
                        },
                        HeightF = 50
                    }
                },
                DataSource = CreateDataSource()
            };
            return report;
        }

        private void simpleButton1_Click(object sender, EventArgs e) {
            // Show the report's Print Preview
            XtraReport1 report = new XtraReport1();
            ReportPrintTool printTool = new ReportPrintTool(report);
            printTool.ShowRibbonPreviewDialog();
        }

        private void simpleButton2_Click(object sender, EventArgs e) {
            // Open the report in an End-User Report Designer
            XtraReport1 report = new XtraReport1();
            ReportDesignTool designTool = new ReportDesignTool(report);
            designTool.ShowRibbonDesignerDialog();
        }

        private void simpleButton3_Click(object sender, EventArgs e) {
            XtraReport report = CreateReport();
            ReportPrintTool printTool = new ReportPrintTool(report);
            printTool.ShowPreviewDialog();
        }

        private void simpleButton4_Click(object sender, EventArgs e) {
            XtraReport1 report = new XtraReport1();

            XPObjectSource dataSource = (XPObjectSource)report.DataSource;
            dataSource.ResolveSession += new EventHandler<ResolveSessionEventArgs>(OnResolveSession);
            dataSource.DismissSession += new EventHandler<ResolveSessionEventArgs>(OnDismissSession);

            ReportPrintTool printTool = new ReportPrintTool(report);
            printTool.ShowPreviewDialog();
        }

        private void OnResolveSession(object sender, ResolveSessionEventArgs e) {
            //Сreate a single instance of IDataLayer if it is not created
            if (DataLayerInstance == null) {
                string connectionString = ConfigurationManager.ConnectionStrings["nwind"].ConnectionString;
                DataLayerInstance = XpoDefault.GetDataLayer(connectionString, DevExpress.Xpo.DB.AutoCreateOption.SchemaAlreadyExists);
            }

            //Create new session based on the instance of DataLayer
            e.Session = new UnitOfWork(DataLayerInstance);
        }

        private void OnDismissSession(object sender, ResolveSessionEventArgs e) {
            e.Session.Session.Dispose();
        }
    }
}
