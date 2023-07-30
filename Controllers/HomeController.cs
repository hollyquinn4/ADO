using ADO.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace ADO.Controllers
{
    public class HomeController : Controller
    {


        public IConfiguration Configuration { get; }
        public HomeController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Inventory inventory)
        {
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"Insert Into Inventory (Name, Price, Quantity) Values ('{inventory.Name}', '{inventory.Price}','{inventory.Quantity}')";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            ViewBag.Result = "Success";
            return View();
        }

        public IActionResult Update(int id)
        {
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];

            Inventory inventory = new Inventory();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"Select * From Inventory Where Id='{id}'";
                SqlCommand command = new SqlCommand(sql, connection);

                connection.Open();

                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        inventory.Id = Convert.ToInt32(dataReader["Id"]);
                        inventory.Name = Convert.ToString(dataReader["Name"]);
                        inventory.Price = Convert.ToDecimal(dataReader["Price"]);
                        inventory.Quantity = Convert.ToInt32(dataReader["Quantity"]);
                        inventory.AddedOn = Convert.ToDateTime(dataReader["AddedOn"]);
                    }
                }

                connection.Close();
            }
            return View(inventory);
        }

        [HttpPost]
        public IActionResult Update(Inventory inventory, int id)
        {
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"Update Inventory SET Name='{inventory.Name}', Price='{inventory.Price}', Quantity='{inventory.Quantity}' Where Id='{id}'";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"Delete From Inventory Where Id='{id}'";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }

            return RedirectToAction("Index");
        }

        public IActionResult Index()
        {
            List<Inventory> inventoryList = new List<Inventory>();

            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DataTable dataTable = new DataTable();

                string sql = "Select * From Inventory";
                SqlCommand command = new SqlCommand(sql, connection);

                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);

                dataAdapter.Fill(dataTable);

                foreach (DataRow dr in dataTable.Rows)
                {
                    Inventory inventory = new Inventory();
                    inventory.Id = Convert.ToInt32(dr["Id"]);
                    inventory.Name = Convert.ToString(dr["Name"]);
                    inventory.Price = Convert.ToDecimal(dr["Price"]);
                    inventory.Quantity = Convert.ToInt32(dr["Quantity"]);
                    inventory.AddedOn = Convert.ToDateTime(dr["AddedOn"]);

                    inventoryList.Add(inventory);
                }

                connection.Close();
            }
            return View(inventoryList);
        }

    

    }
}