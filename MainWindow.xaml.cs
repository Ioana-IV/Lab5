 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AutoLotModel;
using System.Data.Entity;
using System.Data;

namespace Ivanov_Ioana_Lab5
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    enum ActionState
    {
        New,
        Edit,
        Delete,
        Nothing
    }
    public partial class MainWindow : Window
    {
       //using AutoLotModel;
        ActionState action = ActionState.Nothing;
        AutoLotEntitiesModel ctx = new AutoLotEntitiesModel();
        CollectionViewSource customerViewSource;
        CollectionViewSource inventoryViewSource;
        CollectionViewSource customerOrdersViewSource;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            //using System.Data.Entity;
            customerViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerViewSource")));
            customerViewSource.Source = ctx.Customers.Local;
            ctx.Customers.Load();
    
            inventoryViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("inventoryViewSource")));
            inventoryViewSource.Source = ctx.Inventories.Local;
            ctx.Inventories.Load();

            customerOrdersViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerOrdersViewSource")));
            customerOrdersViewSource.Source = ctx.Orders.Local;
            ctx.Orders.Load();

            cmbCustomers.ItemsSource = ctx.Customers.Local;
            cmbCustomers.SelectedValuePath = "CustId";

            cmbInventory.ItemsSource = ctx.Inventories.Local;
            cmbInventory.SelectedValuePath = "CarId";

            BindDataGrid();

        }

        private void BindDataGrid()
        { 
            var queryOrder = from ord in ctx.Orders
                             join cust in ctx.Customers on ord.CustId equals cust.CustId
                             join inv in ctx.Inventories on ord.CarId equals inv.CarId
                             select new { ord.OrderId, ord.CarId, ord.CustId, cust.FirstName, cust.LastName, inv.Make, inv.Color };
            customerOrdersViewSource.Source = queryOrder.ToList();
        }

        // --------------------------------------------------------------------------------------
        //  CUSTOMER CONTROLS
        // --------------------------------------------------------------------------------------

        // these variables will help display the correct name if Edit is cancelled
        String tempFN = null;  //temporare Full Name
        String tempLN = null;  //temporary Last Name

        private void btnPrevCust_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNextCust_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToNext();
        }

        private void btnNewCust_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            customerViewSource.View.MoveCurrentToLast();
            customerViewSource.View.MoveCurrentToNext();
            firstNameTextBox.IsEnabled = true;
            lastNameTextBox.IsEnabled = true;
            btnPrevCust.IsEnabled = false;
            btnNextCust.IsEnabled = false;
            btnNewCust.IsEnabled = false;
            btnEditCust.IsEnabled = false;
            btnDeleteCust.IsEnabled = false;
            btnSaveCust.IsEnabled = true;
            btnCancelCust.IsEnabled = true;
        }

        private void btnEditCust_Click(object sender, RoutedEventArgs e)
        {
            if (firstNameTextBox.Text != String.Empty)
            {
                tempFN = firstNameTextBox.Text;
                tempLN = lastNameTextBox.Text;
                action = ActionState.Edit;
                firstNameTextBox.IsEnabled = true;
                lastNameTextBox.IsEnabled = true;
                btnEditCust.IsEnabled = false;
                btnPrevCust.IsEnabled = false;
                btnNextCust.IsEnabled = false;
                btnNewCust.IsEnabled = false;
                btnDeleteCust.IsEnabled = false;
                btnSaveCust.IsEnabled = true;
                btnCancelCust.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("Please select the entry you want to edit.");
            }

        }

        private void btnDeleteCust_Click(object sender, RoutedEventArgs e)
        {
            if (firstNameTextBox.Text != String.Empty)
            {
                action = ActionState.Delete;
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;
                btnEditCust.IsEnabled = false;
                btnPrevCust.IsEnabled = false;
                btnNextCust.IsEnabled = false;
                btnNewCust.IsEnabled = false;
                btnDeleteCust.IsEnabled = false;
                btnSaveCust.IsEnabled = true;
                btnCancelCust.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("Please select an entry you want to delete.");
            }
        }

        private void btnSaveCust_Click(object sender, RoutedEventArgs e)
        {
            Customer customer = null;
            if (action == ActionState.New)
            {
                try
                {
                    //instatiem Customer entity
                    customer = new Customer()
                    {
                        FirstName = firstNameTextBox.Text.Trim(),
                        LastName = lastNameTextBox.Text.Trim()
                    };

                    //adaugam entitatea nou creata in context
                    ctx.Customers.Add(customer);
                    customerViewSource.View.Refresh();

                    // salvam modificarile
                    ctx.SaveChanges();
                    customerViewSource.View.MoveCurrentToPrevious();
                }
                // using System.Data;
                catch (DataMisalignedException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;
                btnPrevCust.IsEnabled = true;
                btnNextCust.IsEnabled = true;
                btnNewCust.IsEnabled = true;
                btnEditCust.IsEnabled = true;
                btnDeleteCust.IsEnabled = true;
                btnSaveCust.IsEnabled = false;
                btnCancelCust.IsEnabled = false;
                action = ActionState.Nothing;
                
            }
            else if (action == ActionState.Edit)
            {

                try
                {
                    customer = (Customer)customerDataGrid.SelectedItem;
                    customer.FirstName = firstNameTextBox.Text.Trim();
                    customer.LastName = lastNameTextBox.Text.Trim();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                // using System.Data;
                catch (DataMisalignedException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customerViewSource.View.Refresh();
                //pozitionarea pe item-ul curent
                customerViewSource.View.MoveCurrentTo(customer);
                customerViewSource.View.MoveCurrentToPrevious();
                customerViewSource.View.MoveCurrentToNext();
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;
                btnPrevCust.IsEnabled = true;
                btnNextCust.IsEnabled = true;
                btnNewCust.IsEnabled = true;
                btnEditCust.IsEnabled = true;
                btnDeleteCust.IsEnabled = true;
                btnSaveCust.IsEnabled = false;
                btnCancelCust.IsEnabled = false;
                action = ActionState.Nothing;
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    customer = (Customer)customerDataGrid.SelectedItem;
                    ctx.Customers.Remove(customer);
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                // using System.Data;
                catch (DataMisalignedException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customerViewSource.View.Refresh();
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;
                btnPrevCust.IsEnabled = true;
                btnNextCust.IsEnabled = true;
                btnNewCust.IsEnabled = true;
                btnEditCust.IsEnabled = true;
                btnDeleteCust.IsEnabled = true;
                btnSaveCust.IsEnabled = false;
                btnCancelCust.IsEnabled = false;
                action = ActionState.Nothing;
            }

        }

        private void btnCancelCust_Click(object sender, RoutedEventArgs e)
        {
            if (action == ActionState.Edit)
            {
                firstNameTextBox.Text = tempFN;
                lastNameTextBox.Text = tempLN;
                tempFN = null;
                tempLN = null;

            }
            customerViewSource.View.MoveCurrentToPrevious();
            customerViewSource.View.MoveCurrentToNext();
            firstNameTextBox.IsEnabled = false;
            lastNameTextBox.IsEnabled = false;
            btnPrevCust.IsEnabled = true;
            btnNextCust.IsEnabled = true;
            btnNewCust.IsEnabled = true;
            btnEditCust.IsEnabled = true;
            btnDeleteCust.IsEnabled = true;
            btnSaveCust.IsEnabled = false;
            btnCancelCust.IsEnabled = false;
            customerViewSource.View.Refresh();
            action = ActionState.Nothing;
        }

        // --------------------------------------------------------------------------------------
        //  INVENTORY CONTROLS
        // --------------------------------------------------------------------------------------

        // these variables will help display the correct name if Edit is cancelled
        String tempC = null;  //temporare Color
        String tempM = null;  //temporary Make
        private void btnPrevInv_Click(object sender, RoutedEventArgs e)
        {
            inventoryViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNexInv_Click(object sender, RoutedEventArgs e)
        {
            inventoryViewSource.View.MoveCurrentToNext();
        }

        private void btnNewInv_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            inventoryViewSource.View.MoveCurrentToLast();
            inventoryViewSource.View.MoveCurrentToNext();
            colorTextBox.IsEnabled = true;
            makeTextBox.IsEnabled = true;
            btnPrevInv.IsEnabled = false;
            btnNextInv.IsEnabled = false;
            btnNewInv.IsEnabled = false;
            btnEditInv.IsEnabled = false;
            btnDeleteInv.IsEnabled = false;
            btnSaveInv.IsEnabled = true;
            btnCancelInv.IsEnabled = true;
        }

        private void btnEditInv_Click(object sender, RoutedEventArgs e)
        {
            if (colorTextBox.Text != String.Empty)
            {
                tempC = colorTextBox.Text;
                tempM = makeTextBox.Text;
                action = ActionState.Edit;
                colorTextBox.IsEnabled = true;
                makeTextBox.IsEnabled = true;
                btnEditInv.IsEnabled = false;
                btnPrevInv.IsEnabled = false;
                btnNextInv.IsEnabled = false;
                btnNewInv.IsEnabled = false;
                btnDeleteInv.IsEnabled = false;
                btnSaveInv.IsEnabled = true;
                btnCancelInv.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("Please select the entry you want to edit.");
            }

        }

        private void btnDeleteInv_Click(object sender, RoutedEventArgs e)
        {
            if (colorTextBox.Text != String.Empty)
            {
                action = ActionState.Delete;
                colorTextBox.IsEnabled = false;
                colorTextBox.IsEnabled = false;
                btnEditInv.IsEnabled = false;
                btnPrevInv.IsEnabled = false;
                btnNextInv.IsEnabled = false;
                btnNewInv.IsEnabled = false;
                btnDeleteInv.IsEnabled = false;
                btnSaveInv.IsEnabled = true;
                btnCancelInv.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("Please select an entry you want to delete.");
            }
        }

        private void btnSaveInv_Click(object sender, RoutedEventArgs e)
        {
            Inventory inventory = null;
            if (action == ActionState.New)
            {
                try
                {
                    //instatiem Invomer entity
                    inventory = new Inventory()
                    {
                        Color = colorTextBox.Text.Trim(),
                        Make = makeTextBox.Text.Trim()
                    };

                    //adaugam entitatea nou creata in context
                    ctx.Inventories.Add(inventory);
                    inventoryViewSource.View.Refresh();

                    // salvam modificarile
                    ctx.SaveChanges();
                    inventoryViewSource.View.MoveCurrentToPrevious();
                }
                // using System.Data;
                catch (DataMisalignedException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                colorTextBox.IsEnabled = false;
                makeTextBox.IsEnabled = false;
                btnPrevInv.IsEnabled = true;
                btnNextInv.IsEnabled = true;
                btnNewInv.IsEnabled = true;
                btnEditInv.IsEnabled = true;
                btnDeleteInv.IsEnabled = true;
                btnSaveInv.IsEnabled = false;
                btnCancelInv.IsEnabled = false;
                action = ActionState.Nothing;
            }
            else if (action == ActionState.Edit)
            {
                colorTextBox.IsEnabled = true;
                makeTextBox.IsEnabled = true;
                try
                {
                    inventory = (Inventory)inventoryDataGrid.SelectedItem;
                    inventory.Color = colorTextBox.Text.Trim();
                    inventory.Make = makeTextBox.Text.Trim();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                // using System.Data;
                catch (DataMisalignedException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                inventoryViewSource.View.Refresh();
                //pozitionarea pe item-ul curent
                inventoryViewSource.View.MoveCurrentTo(inventory);
                inventoryViewSource.View.MoveCurrentToPrevious();
                inventoryViewSource.View.MoveCurrentToNext();
                colorTextBox.IsEnabled = false;
                makeTextBox.IsEnabled = false;
                btnPrevInv.IsEnabled = true;
                btnNextInv.IsEnabled = true;
                btnNewInv.IsEnabled = true;
                btnEditInv.IsEnabled = true;
                btnDeleteInv.IsEnabled = true;
                btnSaveInv.IsEnabled = false;
                btnCancelInv.IsEnabled = false;
                action = ActionState.Nothing;
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    inventory = (Inventory)inventoryDataGrid.SelectedItem;
                    ctx.Inventories.Remove(inventory);
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                // using System.Data;
                catch (DataMisalignedException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                inventoryViewSource.View.Refresh();
                colorTextBox.IsEnabled = false;
                colorTextBox.IsEnabled = false;
                btnPrevInv.IsEnabled = true;
                btnNextInv.IsEnabled = true;
                btnNewInv.IsEnabled = true;
                btnEditInv.IsEnabled = true;
                btnDeleteInv.IsEnabled = true;
                btnSaveInv.IsEnabled = false;
                btnCancelInv.IsEnabled = false;
                action = ActionState.Nothing;
            }

        }

        private void btnCancelInv_Click(object sender, RoutedEventArgs e)
        {
            if (action == ActionState.Edit)
            {
                colorTextBox.Text = tempC;
                makeTextBox.Text = tempM;
                tempC = null;
                tempM = null;

            }
            inventoryViewSource.View.MoveCurrentToPrevious();
            inventoryViewSource.View.MoveCurrentToNext();
            colorTextBox.IsEnabled = false;
            makeTextBox.IsEnabled = false;
            btnPrevInv.IsEnabled = true;
            btnNextInv.IsEnabled = true;
            btnNewInv.IsEnabled = true;
            btnEditInv.IsEnabled = true;
            btnDeleteInv.IsEnabled = true;
            btnSaveInv.IsEnabled = false;
            btnCancelInv.IsEnabled = false;
            inventoryViewSource.View.Refresh();
            action = ActionState.Nothing;
        }



        // --------------------------------------------------------------------------------------
        //  ORDERS CONTROLS
        // --------------------------------------------------------------------------------------

        // these variables will help display the correct name if Edit is cancelled
        String tempCa = null;  //temporare CarId
        String tempCu = null;  //temporary CustId
        private void btnPrevOrd_Click(object sender, RoutedEventArgs e)
        {
            customerOrdersViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNextOrd_Click(object sender, RoutedEventArgs e)
        {
            customerOrdersViewSource.View.MoveCurrentToNext();
        }

        private void btnNewOrd_Click(object sender, RoutedEventArgs e)
        {

            if ((cmbCustomers.SelectedIndex >-1 ) && (cmbInventory.SelectedIndex >-1))
            {
                action = ActionState.New;

                btnPrevOrd.IsEnabled = false;
                btnNextOrd.IsEnabled = false;
                btnNewOrd.IsEnabled = false;
                btnEditOrd.IsEnabled = false;
                btnDeleteOrd.IsEnabled = false;
                btnSaveOrd.IsEnabled = true;
                btnCancelOrd.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("Please make sure you have selected a Customer and a Car from the dropdown menus.");
            }
        }

        private void btnEditOrd_Click(object sender, RoutedEventArgs e)
        {
            if ((cmbCustomers.SelectedIndex > -1) && (cmbInventory.SelectedIndex > -1))
            {
                tempCa = orderCarIdTextBox.Text;
                tempCu = orderCustIdTextBox.Text;
                action = ActionState.Edit;
                orderCarIdTextBox.IsEnabled = true;
                orderCustIdTextBox.IsEnabled = true;
                btnEditOrd.IsEnabled = false;
                btnPrevOrd.IsEnabled = false;
                btnNextOrd.IsEnabled = false;
                btnNewOrd.IsEnabled = false;
                btnDeleteOrd.IsEnabled = false;
                btnSaveOrd.IsEnabled = true;
                btnCancelOrd.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("Please make sure you have selected a Customer and a Car from the dropdown menus.");
            }
        }

        private void btnDeleteOrd_Click(object sender, RoutedEventArgs e)
        {
            if ((cmbCustomers.SelectedIndex > -1) && (cmbInventory.SelectedIndex > -1))
            {
                action = ActionState.Delete;
                btnEditOrd.IsEnabled = false;
                btnPrevOrd.IsEnabled = false;
                btnNextOrd.IsEnabled = false;
                btnNewOrd.IsEnabled = false;
                btnDeleteOrd.IsEnabled = false;
                btnSaveOrd.IsEnabled = true;
                btnCancelOrd.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("Please make sure you have selected a Customer and a Car from the dropdown menus.");
            }
        }

        private void btnSaveOrd_Click(object sender, RoutedEventArgs e)
        {
            Order order = null;
            if (action == ActionState.New)
            {
                try
                {
                    Customer customer = (Customer)cmbCustomers.SelectedItem;
                    Inventory inventory = (Inventory)cmbInventory.SelectedItem;

                    //instantiem Order entity
                    order = new Order()
                    {
                        CustId = customer.CustId,
                        CarId = inventory.CarId
                    };

                    // adaugam entitatea now creata in context
                    ctx.Orders.Add(order);
 
                    //salvam modificarile              
                    ctx.SaveChanges();
                    BindDataGrid();
                }
                // using System.Data;
                catch (DataMisalignedException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                //orderCarIdTextBox.IsEnabled = false;
                //orderCustIdTextBox.IsEnabled = false;
                btnPrevOrd.IsEnabled = true;
                btnNextOrd.IsEnabled = true;
                btnNewOrd.IsEnabled = true;
                btnEditOrd.IsEnabled = true;
                btnDeleteOrd.IsEnabled = true;
                btnSaveOrd.IsEnabled = false;
                btnCancelOrd.IsEnabled = false;
                action = ActionState.Nothing;
            }
            else if (action == ActionState.Edit)
            {
                dynamic selectedOrder = ordersDataGrid.SelectedItem;
                orderCarIdTextBox.IsEnabled = true;
                orderCustIdTextBox.IsEnabled = true;
                try
                {
                    int curr_id = selectedOrder.OrderId;
                    var editedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId == curr_id);
                    if (editedOrder != null)
                    {
                        editedOrder.CustId = Int32.Parse(cmbCustomers.SelectedValue.ToString());
                        editedOrder.CarId = Int32.Parse(cmbInventory.SelectedValue.ToString());
                    }
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                // using System.Data;
                catch (DataMisalignedException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                BindDataGrid();
                //pozitionarea pe item-ul curent
                customerOrdersViewSource.View.MoveCurrentTo(selectedOrder);
                customerOrdersViewSource.View.MoveCurrentToPrevious();
                customerOrdersViewSource.View.MoveCurrentToNext();
                orderCarIdTextBox.IsEnabled = false;
                orderCustIdTextBox.IsEnabled = false;
                btnPrevOrd.IsEnabled = true;
                btnNextOrd.IsEnabled = true;
                btnNewOrd.IsEnabled = true;
                btnEditOrd.IsEnabled = true;
                btnDeleteOrd.IsEnabled = true;
                btnSaveOrd.IsEnabled = false;
                btnCancelOrd.IsEnabled = false;
                action = ActionState.Nothing;
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    dynamic selectedOrder = ordersDataGrid.SelectedItem;
                    int curr_id = selectedOrder.OrderId;
                    var deletedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId == curr_id);
                    if (deletedOrder != null)
                    {
                        ctx.Orders.Remove(deletedOrder);
                        ctx.SaveChanges();
                        MessageBox.Show("Order Deleted Successfully", "Message");
                        BindDataGrid();
                    }
                }
                // using System.Data;
                catch (DataMisalignedException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customerOrdersViewSource.View.Refresh();
                orderCarIdTextBox.IsEnabled = false;
                orderCustIdTextBox.IsEnabled = false;
                btnPrevOrd.IsEnabled = true;
                btnNextOrd.IsEnabled = true;
                btnNewOrd.IsEnabled = true;
                btnEditOrd.IsEnabled = true;
                btnDeleteOrd.IsEnabled = true;
                btnSaveOrd.IsEnabled = false;
                btnCancelOrd.IsEnabled = false;
                action = ActionState.Nothing;
            }

        }

        private void btnCancelOrd_Click(object sender, RoutedEventArgs e)
        {
            if (action == ActionState.Edit)
            {
                orderCarIdTextBox.Text = tempCa;
                orderCustIdTextBox.Text = tempCu;
                tempCa = null;
                tempCu = null;

            }
            customerOrdersViewSource.View.MoveCurrentToPrevious();
            customerOrdersViewSource.View.MoveCurrentToNext();
            orderCarIdTextBox.IsEnabled = false;
            orderCustIdTextBox.IsEnabled = false;
            btnPrevOrd.IsEnabled = true;
            btnNextOrd.IsEnabled = true;
            btnNewOrd.IsEnabled = true;
            btnEditOrd.IsEnabled = true;
            btnDeleteOrd.IsEnabled = true;
            btnSaveOrd.IsEnabled = false;
            btnCancelOrd.IsEnabled = false;
            customerOrdersViewSource.View.Refresh();
            action = ActionState.Nothing;
        }

    }


}
