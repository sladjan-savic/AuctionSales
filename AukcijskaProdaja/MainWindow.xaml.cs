using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Linq;
using System.Data.SqlClient;
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
using System.Data.Entity;
using System.Timers;
using System.Windows.Threading;

namespace AukcijskaProdaja
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       public Products newProduct { get; set; }
        public Users newUser { get; set; }
        public decimal I { get => i; set => i = value; }

        DispatcherTimer timer = new DispatcherTimer();


        AuctionSalesEntities context = new AuctionSalesEntities();
        CollectionViewSource productViewResource;
        CollectionViewSource auctionViewResource;
        CollectionViewSource userViewResource;

    

        public MainWindow()
        {
            

            InitializeComponent();
            newProduct = new Products();
            newUser = new Users();
            productViewResource = ((CollectionViewSource)(FindResource("productsViewSource")));
            auctionViewResource = ((CollectionViewSource)(FindResource("auctionsViewSource")));
            userViewResource = ((CollectionViewSource)FindResource("usersViewSource"));           
            DataContext = this;

            timer.Tick += new EventHandler(Timer_tick);
            timer.Interval = new TimeSpan(0, 0, 1);
                                         
        }


        
       private void UserChanged()
        {
            string usrName = userNameComboBox.Text;

            if (usrName=="UserGuest")
            {
                btnBid.IsEnabled = false;
                btnDelete.IsEnabled = false;
                btnNew.IsEnabled = false;
            }
           else if (usrName=="UserNormal")
            {
                btnDelete.IsEnabled = false;
                btnNew.IsEnabled = false;
                btnBid.IsEnabled = true;
            }
            else if (usrName == "UserAdmin")
            {
                btnBid.IsEnabled = true;
                btnDelete.IsEnabled = true;
                btnNew.IsEnabled = true;
            }
            
           
        }

        int tick = 120;
        private void Timer_tick(object sender,EventArgs e)
        {
            tick--;
            labelTicker.Content = tick.ToString();

            if (tick ==0)
            {
                timer.Stop();
                using (AuctionSalesEntities auctionEntity = new AuctionSalesEntities())
                {
                    Auctions auction = new Auctions
                    {
                        ProductName = productNameTextBox.Text,
                        UnitPrice = Convert.ToDecimal(unitPriceTextBox.Text),
                        LastBid = Convert.ToDecimal(lastBidTextBox.Text),
                        LastBidder = lastBidderTextBox.Text
                    
                    };
                    auctionEntity.Auctions.Add(auction);
                    auctionEntity.SaveChanges();                    
                    MessageBox.Show("Auction for item " + textBoxProductName.Text + " is over." + "\n"+"Auction winner is "+lastBidderTextBox.Text+".");
                    auctionViewResource.View.Refresh();
                }
            }
        }


        private void RestartTimer()
        {
             tick = 120;
            timer.Start();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            System.Windows.Data.CollectionViewSource productsViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("productsViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            // productsViewSource.Source = [generic data source]
            System.Windows.Data.CollectionViewSource auctionsViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("auctionsViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            // auctionsViewSource.Source = [generic data source]
            System.Windows.Data.CollectionViewSource usersViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("usersViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            // auctionsViewSource.Source = [generic data source]
            context.Products.Load();
            context.Auctions.Load();
            context.Users.Load();
            productsViewSource.Source = context.Products.Local;
            auctionsViewSource.Source = context.Auctions.Local;
            userViewResource.Source = context.Users.Local;
          
        }


              
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            
            try
            {
                newProduct.ProductName = textBoxProductName.Text;
                newProduct.UnitPrice = Convert.ToDecimal(textBoxNewUnitPrice.Text);
                context.Products.Add(newProduct);
                productViewResource.View.Refresh();
                context.SaveChanges();
                textBoxNewUnitPrice.Clear();
                textBoxProductName.Clear();
            }
            catch 
            {
                MessageBox.Show("Both fields are required");
            }
                      
        }

       
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
           
            var cur = productViewResource.View.CurrentItem as Products;

            var prod = (from p in context.Products where p.ProductName == cur.ProductName select p).FirstOrDefault();

            context.Products.Remove(prod);
            context.SaveChanges();
            productViewResource.View.Refresh();
                     
        }

        static decimal i= 1;
        private void btnBid_Click(object sender, RoutedEventArgs e)
        {
            
            RestartTimer();          
            timer.Start();            
            lastBidderTextBox.Clear();
            lastBidderTextBox.AppendText(userNameComboBox.Text);
            lastBidTextBox.Clear();           
            lastBidTextBox.Text = (decimal.Parse(unitPriceTextBox.Text) + i).ToString();
            i++;                               
        }

        private void userNameComboBox_DropDownClosed(object sender, EventArgs e)
        {
            UserChanged();
        }

        private void btnResetUI_Click(object sender, RoutedEventArgs e)
        {
            textBoxProductName.Clear();
            textBoxNewUnitPrice.Clear();
            productNameTextBox.Clear();
            unitPriceTextBox.Clear();
            lastBidTextBox.Clear();
            lastBidderTextBox.Clear();
        }
    }
}
