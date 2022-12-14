using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;
using GoWMS.Server.Controllers;
using GoWMS.Server.Models;
using GoWMS.Server.Models.Erp;
using GoWMS.Server.Models.Api;
using GoWMS.Server.Models.Inb;
using System.Text;

namespace GoWMS.Server.Data
{
    public class ErpDAL
    {
        readonly private string connectionString = ConnGlobals.GetConnLocalDBPG();

        public IEnumerable<V_Receiving_OrdersInfo> GetAllErpReceivingOrders()
        {
            List<V_Receiving_OrdersInfo> lstobj = new List<V_Receiving_OrdersInfo>();
            using (NpgsqlConnection con = new NpgsqlConnection(connectionString))
            {
                StringBuilder sql = new StringBuilder();

                //sql.AppendLine("SELECT Package_ID,Roll_ID");
                //sql.AppendLine(",Material_Code,Material_Description");
                //sql.AppendLine(",Receiving_Date,GR_Quantity");
                //sql.AppendLine(",Unit,GR_Quantity_KG");
                //sql.AppendLine(",WH_Code,Warehouse,Job,Job_Cod");
                //sql.AppendLine("FROM dbo.V_Receiving_Orders");


                sql.AppendLine("SELECT lenum as Package_ID, lenum as Roll_ID");
                sql.AppendLine(",matnr as Material_Code, '-' as Material_Description");
                //sql.AppendLine(",SUBSTRING (matnr FROM 2) as Material_Code, '-' as Material_Description");
                sql.AppendLine(",created as Receiving_Date, matqty as  GR_Quantity");
                sql.AppendLine(",null as Unit,1 as GR_Quantity_KG");
                sql.AppendLine(",lgnum as  WH_Code, lgnum as Warehouse, matnr as Job, matbatch as Job_Code");
                sql.AppendLine("FROM api.postasrsorders");

                NpgsqlCommand cmd = new NpgsqlCommand(sql.ToString(), con)

                {
                    CommandType = CommandType.Text
                };

                con.Open();
                NpgsqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    V_Receiving_OrdersInfo objrd = new V_Receiving_OrdersInfo
                    {
                        Package_ID= rdr["Package_ID"].ToString(),
                        Roll_ID= rdr["Roll_ID"].ToString(),
                        Material_Code= rdr["Material_Code"].ToString(),
                        Material_Description= rdr["Material_Description"].ToString(),
                        Receiving_Date= rdr["Receiving_Date"] == DBNull.Value ? null : (DateTime?)rdr["Receiving_Date"],
                        GR_Quantity= rdr["GR_Quantity"] == DBNull.Value ? null : (decimal?)rdr["GR_Quantity"],
                        Unit = rdr["Unit"].ToString(),
                        GR_Quantity_KG= rdr["GR_Quantity_KG"] == DBNull.Value ? null : (decimal?)rdr["GR_Quantity_KG"],
                        WH_Code= rdr["WH_Code"].ToString(),
                        Warehouse= rdr["Warehouse"].ToString(),
                        Location= rdr["Location"].ToString(),
                        Document_Number= rdr["Document_Number"].ToString(),
                        Job= rdr["Job"].ToString(),
                        Job_Code= rdr["Job_Code"].ToString()
                    };
                    lstobj.Add(objrd);
                }
                con.Close();
            }
            return lstobj;
        }

        public IEnumerable<Api_Receivingorders_Go> GetErpReceivingOrdersByTag(string Tag)
        {
            List<Api_Receivingorders_Go> lstobj = new List<Api_Receivingorders_Go>();
            using (NpgsqlConnection con = new NpgsqlConnection(connectionString))
            {
                StringBuilder sql = new StringBuilder();

                //sql.AppendLine("SELECT Package_ID,Roll_ID");
                //sql.AppendLine(",Material_Code,Material_Description");
                //sql.AppendLine(",Receiving_Date,GR_Quantity");
                //sql.AppendLine(",Unit,GR_Quantity_KG");
                //sql.AppendLine(",WH_Code,Warehouse,Job,Job_Cod");
                //sql.AppendLine("FROM dbo.V_Receiving_Orders");
                //sql.AppendLine("WHERE Package_ID = @tag");


                sql.AppendLine("SELECT lenum as Package_ID, lenum as Roll_ID");
                sql.AppendLine(", matnr as Material_Code, '-' as Material_Description");
                //sql.AppendLine(",SUBSTRING (matnr FROM 2) as Material_Code, '-' as Material_Description");
                sql.AppendLine(",created as Receiving_Date, cast(matqty as numeric) as  GR_Quantity");
                sql.AppendLine(",null as Unit, 1.0 as GR_Quantity_KG");
                sql.AppendLine(",lgnum as  WH_Code, lgnum as Warehouse, matnr as Job, matbatch as Job_Code , karor as Document_Number , kxbin as Location ");
                sql.AppendLine("FROM api.postasrsorders");
                sql.AppendLine("WHERE lenum = @tag");
                sql.AppendLine("AND typor = @typor");
                sql.AppendLine("Limit 1");


                NpgsqlCommand cmd = new NpgsqlCommand(sql.ToString(), con)
                {
                    CommandType = CommandType.Text
                };

                con.Open();
                cmd.Parameters.AddWithValue("@tag", Tag);
                cmd.Parameters.AddWithValue("@typor", "PUT");
                NpgsqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Api_Receivingorders_Go objrd = new Api_Receivingorders_Go
                    {
                        Package_Id = rdr["Package_ID"].ToString(),
                        Roll_Id = rdr["Roll_ID"].ToString(),
                        Material_Code = rdr["Material_Code"].ToString(),
                        Material_Description = rdr["Material_Description"].ToString(),
                        Receiving_Date = rdr["Receiving_Date"] == DBNull.Value ? null : (DateTime?)rdr["Receiving_Date"],
                        Gr_Quantity = rdr["GR_Quantity"] == DBNull.Value ? null : (decimal?)rdr["GR_Quantity"],
                        Unit = rdr["Unit"].ToString(),
                        Gr_Quantity_Kg = rdr["GR_Quantity_KG"] == DBNull.Value ? null : (decimal?)rdr["GR_Quantity_KG"],
                        Wh_Code = rdr["WH_Code"].ToString(),
                        Warehouse = rdr["Warehouse"].ToString(),
                        Locationno = rdr["Location"].ToString(),
                        Document_Number = rdr["Document_Number"].ToString(),
                        Job = rdr["Job"].ToString(),
                        Job_Code = rdr["Job_Code"].ToString(),
                        Matcategory = "01"
                    };
                    lstobj.Add(objrd);
                }
                con.Close();
            }
            return lstobj;
        }


        public IEnumerable<V_CylinderInfo> GetAllErpCylinders()
        {
            List<V_CylinderInfo> lstobj = new List<V_CylinderInfo>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT Material,Material_Description " +
                    ",Customer_Code,Customer_Description,Customer_Reference" +
                    ",Color_1,Cylinder1" +
                    ",Color_2,Cylinder2" +
                    ",Color_3,Cylinder3" +
                    ",Color_4,Cylinder4" +
                    ",Color_5,Cylinder5" +
                    ",Color_6,Cylinder6" +
                    ",Color_7,Cylinder7" +
                    ",Color_8,Cylinder8" +
                    ",Color_9,Cylinder9" +
                    ",Color_10,Cylinder10 " +
                    "FROM dbo.V_Cylinder", con)
                {
                    CommandType = CommandType.Text
                };

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    V_CylinderInfo objrd = new V_CylinderInfo
                    {
                        Material = rdr["Material"].ToString(),
                        Material_Description= rdr["Material_Description"].ToString(),
                        Customer_Code= rdr["Customer_Code"].ToString(),
                        Customer_Description= rdr["Customer_Description"].ToString(),
                        Customer_Reference= rdr["Customer_Reference"].ToString(),
                        Color_1= rdr["Color_1"].ToString(),
                        Cylinder1= rdr["Cylinder1"].ToString(),
                        Color_2= rdr["Color_2"].ToString(),
                        Cylinder2= rdr["Cylinder2"].ToString(),
                        Color_3= rdr["Color_3"].ToString(),
                        Cylinder3= rdr["Cylinder3"].ToString(),
                        Color_4= rdr["Color_4"].ToString(),
                        Cylinder4= rdr["Cylinder4"].ToString(),
                        Color_5= rdr["Color_5"].ToString(),
                        Cylinder5= rdr["Cylinder5"].ToString(),
                        Color_6= rdr["Color_6"].ToString(),
                        Cylinder6= rdr["Cylinder6"].ToString(),
                        Color_7= rdr["Color_7"].ToString(),
                        Cylinder7= rdr["Cylinder7"].ToString(),
                        Color_8= rdr["Color_8"].ToString(),
                        Cylinder8= rdr["Cylinder8"].ToString(),
                        Color_9= rdr["Color_9"].ToString(),
                        Cylinder9= rdr["Cylinder9"].ToString(),
                        Color_10= rdr["Color_10"].ToString(),
                        Cylinder10= rdr["Cylinder10"].ToString()
                    };
                    lstobj.Add(objrd);
                }
                con.Close();
            }
            return lstobj;
        }


        public async Task<IEnumerable<V_List_OF_Materials_NeedsInfo>> GetAllErpListofNeeds()
        {
            List<V_List_OF_Materials_NeedsInfo> lstobj = new List<V_List_OF_Materials_NeedsInfo>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT Customer,Customer_Name" +
                    ",Finished_Product,Finished_Product_Description" +
                    ",Material_Code,Description" +
                    ",Element,cast(Quantity AS DECIMAL(18,4)) AS Quantity" +
                    ",Unit,Job" +
                    ",Job_Code,MO_Barcode " +
                    "FROM dbo.V_List_OF_Materials_Need", con)
                {
                    CommandType = CommandType.Text
                };

                await con.OpenAsync();

                var rdr = await cmd.ExecuteReaderAsync();

                //SqlDataReader rdr = cmd.ExecuteReader();
                //Task ReadSQL = await rdr.ReadAsync();

                while (await rdr.ReadAsync())
                {
                    V_List_OF_Materials_NeedsInfo objrd = new V_List_OF_Materials_NeedsInfo
                    {
                        Customer=rdr["Customer"].ToString(),
                        Customer_Name=rdr["Customer_Name"].ToString(),
                        Finished_Product= rdr["Finished_Product"].ToString(),
                        Finished_Product_Description= rdr["Finished_Product_Description"].ToString(),
                        Material_Code= rdr["Material_Code"].ToString(),
                        Description= rdr["Description"].ToString(),
                        Element= rdr["Element"].ToString(),
                        Quantity= rdr["Quantity"] == DBNull.Value ? null : (decimal?)rdr["Quantity"],
                        Unit = rdr["Unit"].ToString(),
                        Job= rdr["Job"].ToString(),
                        Job_Code= rdr["Job_Code"].ToString(),
                        MO_Barcode= rdr["MO_Barcode"].ToString()
                    };
                    lstobj.Add(objrd);
                }
                await con.CloseAsync();
            }
            return lstobj;
        }

        public IEnumerable<Api_Deliveryorder_Go> GetAllErpListofNeedsByMo(string mocode)
        {
            List<Api_Deliveryorder_Go> lstobj = new List<Api_Deliveryorder_Go>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT Customer,Customer_Name" +
                    ",Finished_Product,Finished_Product_Description" +
                    ",Material_Code,Description" +
                    ",Element,cast(Quantity AS DECIMAL(18,4)) AS Quantity" +
                    ",Unit,Job" +
                    ",Job_Code,MO_Barcode " +
                    "FROM dbo.V_List_OF_Materials_Need " +
                    "WHERE MO_Barcode = '" + mocode + "'", con)
                {
                    CommandType = CommandType.Text
                };

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Api_Deliveryorder_Go objrd = new Api_Deliveryorder_Go
                    {
                        Package_Id = "",
                        Roll_Id = "",
                        Customer_Code = rdr["Customer"].ToString(),
                        Customer_Description = rdr["Customer_Name"].ToString(),
                        Finished_Product = rdr["Finished_Product"].ToString(),
                        Finished_Product_Description = rdr["Finished_Product_Description"].ToString(),
                        Material_Code = rdr["Material_Code"].ToString(),
                        Material_Description = rdr["Description"].ToString(),
                        Matelement = rdr["Element"].ToString(),
                        Wh_Code = "",
                        Warehouse = "",
                        Locationno = "",
                        Quantity = rdr["Quantity"] == DBNull.Value ? null : (decimal?)rdr["Quantity"],
                        Unit = rdr["Unit"].ToString(),
                        Job = rdr["Job"].ToString(),
                        Job_Code = rdr["Job_Code"].ToString(),
                        Mo_Barcode = rdr["MO_Barcode"].ToString(),
                        Picked = 0,
                        Dotype = "LON"

                    };
                    lstobj.Add(objrd);
                }
                con.Close();
            }
            return lstobj;
        }


        public IEnumerable<V_Reserved_MaterialsInfo> GetAllErpReservedMaterials()
        {
            List<V_Reserved_MaterialsInfo> lstobj = new List<V_Reserved_MaterialsInfo>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT Package_ID,Roll_ID " +
                    ",Material_Code,Description" +
                    ",WH_Code,Warehouse" +
                    ",Location,cast(Quantity as float) AS Quantity" +
                    ",Unit,Job" +
                    ",Job_Code,MO_Barcode " +
                    "FROM dbo.V_Reserved_Materials", con)
                {
                    CommandType = CommandType.Text
                };

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    V_Reserved_MaterialsInfo objrd = new V_Reserved_MaterialsInfo
                    {
                        Package_ID = rdr["Package_ID"].ToString(),
                        Roll_ID = rdr["Roll_ID"].ToString(),
                        Material_Code= rdr["Material_Code"].ToString(),
                        Description = rdr["Description"].ToString(),
                        WH_Code=rdr["WH_Code"].ToString(),
                        Warehouse= rdr["Warehouse"].ToString(),
                        Location= rdr["Location"].ToString(),
                        Quantity = rdr["Quantity"] == DBNull.Value ? null : (double?)rdr["Quantity"],
                        Unit = rdr["Unit"].ToString(),
                        Job = rdr["Job"].ToString(),
                        Job_Code = rdr["Job_Code"].ToString(),
                        MO_Barcode = rdr["MO_Barcode"].ToString()

                    };
                    lstobj.Add(objrd);
                }
                con.Close();
            }
            return lstobj;
        }

        public IEnumerable<Api_Deliveryorder_Go> GetAllErpReservedMaterialsbyMo(string mocode)
        {
            List<Api_Deliveryorder_Go> lstobj = new List<Api_Deliveryorder_Go>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT Package_ID,Roll_ID " +
                    ",Material_Code,Description" +
                    ",WH_Code,Warehouse" +
                    ",Location,cast(Quantity AS DECIMAL(18,4)) as Quantity" + 
                    ",Unit,Job" +
                    ",Job_Code,MO_Barcode " +
                    "FROM dbo.V_Reserved_Materials " +
                    "WHERE MO_Barcode = '" + mocode + "'", con)
                {
                    CommandType = CommandType.Text
                };

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Api_Deliveryorder_Go objrd = new Api_Deliveryorder_Go
                    {
                        Package_Id = rdr["Package_ID"].ToString(),
                        Roll_Id = rdr["Roll_ID"].ToString(),
                        Material_Code = rdr["Material_Code"].ToString(),
                        Material_Description = rdr["Description"].ToString(),
                        Matelement = rdr["Description"].ToString(),
                        Wh_Code = rdr["WH_Code"].ToString(),
                        Warehouse = rdr["Warehouse"].ToString(),
                        Locationno = rdr["Location"].ToString(),
                        Quantity = rdr["Quantity"] == DBNull.Value ? null : (decimal?)rdr["Quantity"],
                        Unit = rdr["Unit"].ToString(),
                        Job = rdr["Job"].ToString(),
                        Job_Code = rdr["Job_Code"].ToString(),
                        Mo_Barcode = rdr["MO_Barcode"].ToString(),
                        Picked = 0,
                        Dotype = "RES",
                        Customer_Code = "",
                        Customer_Description = "",
                        Finished_Product = "",
                        Finished_Product_Description = ""
                    };
                    lstobj.Add(objrd);
                }
                con.Close();
            }
            return lstobj;
        }

        public IEnumerable<MaterialInfo> GetAllErpMatReceivingOrders()
        {
            List<MaterialInfo> lstobj = new List<MaterialInfo>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT " +
                    "Material_Code,Material_Description" +
                    ",Unit " +
                    "FROM dbo.V_Receiving_Orders " +
                    "GROUP BY Material_Code, Material_Description, Unit"
                    , con)
                {
                    CommandType = CommandType.Text
                };

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    MaterialInfo objrd = new MaterialInfo
                    {
                        Material_Code = rdr["Material_Code"].ToString(),
                        Material_Description = rdr["Material_Description"].ToString(),                   
                        Unit = rdr["Unit"].ToString(),
                    };
                    lstobj.Add(objrd);
                }
                con.Close();
            }
            return lstobj;
        }

    }
}
