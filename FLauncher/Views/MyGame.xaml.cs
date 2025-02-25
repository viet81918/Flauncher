﻿using FLauncher.DAO;
using FLauncher.Model;
using FLauncher.Repositories;
using FLauncher.Services;
using FLauncher.Utilities;
using FLauncher.ViewModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FLauncher.Views
{
    /// <summary>
    /// Interaction logic for MyGame.xaml
    /// </summary>
    public partial class MyGame : Window
    {
        private User _user;
        private Gamer _gamer;
        private GamePublisher _gamePublisher;
        private readonly IPublisherRepository _publisherRepo;
        private readonly GamerRepository _gamerRepo;
        private readonly INotiRepository _notiRepo;
        private readonly FriendRepository _friendRepo;
        private readonly IGameRepository _gameRepo;
        private readonly IGenresRepository _genreRepo;
        private FriendService _friendService;
        private readonly ICategoryRepository _categoryRepo;
        private Game _game;

        private readonly IReviewRepository _reviewRepo;

        private readonly IUserRepository _userRepo;

        private readonly CategoryDAO _categoryDAO;

        public MyGame(User user)
        {
            InitializeComponent();
            if (user.Role == 2) // Giả sử 1 là Publisher
            {
                MessageButon.Visibility = Visibility.Collapsed; // Ẩn
                profileButton.Visibility = Visibility.Collapsed;
            }
            else if (user.Role == 3) // Giả sử 2 là Gamer
            {
                MessageButon.Visibility = Visibility.Visible; // Hiện
                profileButton.Visibility = Visibility.Visible;
            }
            _user = user;
            _gamerRepo = new GamerRepository();
            _notiRepo = new NotiRepository();
            _friendRepo = new FriendRepository();
            _gameRepo = new GameRepository();
            _genreRepo = new GenreRepository();
            _publisherRepo = new PublisherRepository();
            _notiRepo = new NotiRepository();
            _categoryRepo = new CategoryRepository();

            _userRepo = new UserRepository();


            _reviewRepo = new ReviewRepository();

            _categoryDAO = CategoryDAO.Instance; // Singleton instance of CategoryDAO



            InitializeData(user);
        }

        private async void InitializeData(User user)
        {
            if (user.Role == 3)
            {
                _gamer = _gamerRepo.GetGamerByUser(_user);
            }
            if (user.Role == 2)
            {
                _gamePublisher = _publisherRepo.GetPublisherByUser(_user);
            }
            // Fetch top games and genres asynchronously

            var genres = await _genreRepo.GetGenres();    // Assuming GetGenres() is async

            if (user.Role == 3) // Role 3 - Gamer
            {

                var games = await _gameRepo.GetGamesByGamer(_gamer);
                
                var unreadNotifications = await _notiRepo.GetUnreadNotiforGamer(_gamer);
                var friendInvitations = await _friendRepo.GetFriendInvitationsForGamer(_gamer);

                var categories = await _categoryRepo.GetAllCategoriesByGamerAsync(_gamer);
                DataContext = new MyGameViewModel(_gamer, unreadNotifications, friendInvitations, games, categories);

            }
            else if (user.Role == 2) // Role 2 - Publisher
            {
               // _gamePublisher = _publisherRepo.GetPublisherByUser(user); // Assuming async

                var gamesPub = await _gameRepo.GetGamesByPublisher(_gamePublisher);
              
                DataContext = new MyGameViewModel(_gamePublisher, gamesPub);
            }

        }
        private async void InitializeGamerData(Game game, Model.User user)
        {
            _game = game;
            if (user.Role == 3)
            {
                _gamer = _gamerRepo.GetGamerByUser(user);
            }
            else
            {
                _gamePublisher = _publisherRepo.GetPublisherByUser(user);
            }



            var genres = await _genreRepo.GetGenresFromGame(game); // Get genres from your repository
            var reviews = await _reviewRepo.GetReviewsByGame(game); // Get reviews from your repository
            var publisher = await _publisherRepo.GetPublisherByGame(game);
            var updates = await _publisherRepo.getUpdatesForGame(game);

            // Set the DataContext to your ViewModel            
            if (_gamer != null)
            {
                var games = await _gameRepo.GetGamesByGamer(_gamer);
                var friendwithsamegame = await _friendRepo.GetFriendWithTheSameGame(game, _gamer);
                var unreadNotifications = await _notiRepo.GetUnreadNotiforGamer(_gamer);
                var friendInvitations = await _friendRepo.GetFriendInvitationsForGamer(_gamer);
                var Achivements = await _gameRepo.GetAchivesFromGame(_game);
                var Unlock = await _gameRepo.GetUnlockAchivementsFromGame(Achivements, _gamer);
                var UnlockAchivements = await _gameRepo.GetAchivementsFromUnlocks(Unlock);
                var LockAchivements = await _gameRepo.GetLockAchivement(Achivements, _gamer);
                var reviewers = await _gamerRepo.GetGamersFromGame(game);
                var isBuy = await _gameRepo.IsBuyGame(game, _gamer);
                var isUpdate = await _gamerRepo.IsUpdate(game, _gamer);
                var isDownLoad = await _gameRepo.isDownload(game, _gamer);

                var categories = await _categoryRepo.GetAllCategoriesByGamerAsync(_gamer);
                DataContext = new MyGameViewModel(game, _gamer, genres, reviews, unreadNotifications, friendInvitations, publisher, updates, friendwithsamegame, UnlockAchivements, Achivements, LockAchivements, Unlock, reviewers, isBuy, isDownLoad, isUpdate, games, categories);

            }
            else if (_gamePublisher != null)
            {
                var gamesPub = await _gameRepo.GetGamesByPublisher(_gamePublisher);
                var isPublish = await _gameRepo.IsPublishGame(game, _gamePublisher);
                var gamers = await _gamerRepo.GetGamersFromGame(game);
                var Achivements = await _gameRepo.GetAchivesFromGame(_game);

                DataContext = new MyGameViewModel(game, genres, reviews, publisher, updates, isPublish, Achivements, gamers, gamesPub);
            }
        }

        private void Polygon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //To move the window on mouse down
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
        private void Uninstall_Click(object sender, RoutedEventArgs e)
        {
            _gameRepo.Uninstall_Game(_gamer, _game);
        }

        private void minimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void maximizeButton_Click(object sender, RoutedEventArgs e)
        {
            //First detect if windows is in normal state or maximized
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else
                WindowState = WindowState.Normal;
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            //Close the App
            Close();
        }
        private void Download_Click(object sender, RoutedEventArgs e)
        {
            string location = Microsoft.VisualBasic.Interaction.InputBox("Enter the location your want to download the game :", "Download");

            if (string.IsNullOrWhiteSpace(location))
            {
                System.Windows.MessageBox.Show("Please enter a valid Location.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _gameRepo.Download_game(_game, location, _gamer);
        }
        private void Play_Click(object sender, RoutedEventArgs e)
        {

            _gameRepo.Play_Game(_game, _gamer);
        }
        private async void AddCategory_Click(object sender, RoutedEventArgs e)
        {
            // Show the InputBox to prompt the user for the category name
            string categoryName = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter the name of the new category:", // Prompt message
                "Add Category",                       // Title of the dialog
                string.Empty                          // Default value (empty string)
            );

            // Check if the user entered a valid name (not empty or whitespace)
            if (!string.IsNullOrWhiteSpace(categoryName))
            {
                // Create a new Category object
                var newCategory = new Category
                {
                    NameCategories = categoryName,
                    // Add other properties as necessary, such as GamerId
                };

                // Add the new category to the Categories collection in your ViewModel
                if (DataContext is MyGameViewModel viewModel)
                {
                    newCategory = await _categoryRepo.AddCategoryAsync(_gamer, categoryName);
                    viewModel.Categories.Add(newCategory);
                 
                }
            }
            else
            {
                // Optionally, show an error message if the input is invalid
                MessageBox.Show("Category name cannot be empty.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void DeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            // Get the categories collection from the ViewModel (assumed you are using MyGameViewModel)
            if (DataContext is MyGameViewModel viewModel)
            {
                var deleteCategoryWindow = new CategorySelection(viewModel.Categories);
                deleteCategoryWindow.ShowDialog(); // Show the window as a dialog, so the user can interact with it
            }
        }
        private void AddGameToCategory_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MyGameViewModel viewModel && viewModel.Game != null)
            {
                // Open the CategorySelection window for adding a game to a category
                var categorySelectionWindow = new CategorySelection(viewModel.Categories, viewModel.Game);
                categorySelectionWindow.ShowDialog();

                // Refresh the Categories collection in the view model
                viewModel.RefreshCategories();
            }
        }

        private void RemoveGameFromCategory_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MyGameViewModel viewModel && viewModel.Game != null)
            {
                // Open the CategorySelection window for removing a game from a category
                var categorySelectionWindow = new CategorySelection(viewModel.Categories, viewModel.Game, viewModel.Gamer);
                categorySelectionWindow.ShowDialog();

                // Refresh the Categories collection in the view model
                viewModel.RefreshCategories();
            }
        }


        private void Update_Click(object sender, RoutedEventArgs e)
        {
            // Bước 1: Tạo OpenFileDialog để chọn file
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "All Files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true) // Đúng kiểu bool
            {
                // Lấy đường dẫn file đã chọn
                string selectedFilePath = openFileDialog.FileName;

                // Bước 2: Hiển thị InputBox để người dùng nhập message
                string message = Microsoft.VisualBasic.Interaction.InputBox(
                    "Please enter your message:",
                    "Input Message",
                    "Default message here...",
                    -1, -1
                );

                if (string.IsNullOrWhiteSpace(message))
                {
                    System.Windows.MessageBox.Show("Message is empty or cancelled!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Gọi hàm cập nhật game
                _gameRepo.Upload_game(_gamePublisher, _game, selectedFilePath, message);
            }
        }
    



        private void Reinstall_Click(object sender, RoutedEventArgs e)
        {
            _gameRepo.Reinstall(_game, _gamer);
        }
        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            MyGame seriously = new MyGame(_user);
            seriously.Show();
            this.Close();
        }
        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchTextBox.Text == "Search name game")
            {
                SearchTextBox.Text = string.Empty;
                SearchTextBox.Foreground = (System.Windows.Media.Brush)System.Windows.Application.Current.Resources["SecondaryBrush"];
            }
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                SearchTextBox.Text = "Search name game";
            }
        }
        private void ReinstallGame_click(object sender, RoutedEventArgs e)
        {
            _gameRepo.Reinstall(_game, _gamer);
        }
        private void Tracking_Click(object sender, RoutedEventArgs e)
        {
            var gameDetailPage = new TrackingTimePlayed(_gamer, _game);
            gameDetailPage.Show();
        }

        private void Achivement_Click(object sender, RoutedEventArgs e)
        {
            var gameDetailPage = new AchivementManagement(_user, _game);
            gameDetailPage.Show();
            this.Close();
        }
        private void TrackingPlayers_Click(object sender, RoutedEventArgs e)
        {
            var gameDetailPage = new TrackingNumberPlayer(_game);
            gameDetailPage.Show();
        }
        private void messageButton_Click(Object sender, MouseButtonEventArgs e)

        {
            var currentGamer = _gamer;
            MessageWindow messWindow = new MessageWindow(currentGamer);
            messWindow.Show();

            this.Hide();
            this.Close();
        }
        private void logoutButton_Click(object sender, MouseButtonEventArgs e)
        {
            var result = MessageBox.Show("Bạn muốn đăng xuất?", "Xác nhận đăng xuất", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                SessionManager.ClearSession();
                DeleteLoginInfoFile();
                this.Hide();
                Login login = new Login();
                login.Show();

                this.Close();
            }
        }

        private void DeleteLoginInfoFile()
        {
            string appDataPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FLauncher");
            string jsonFilePath = System.IO.Path.Combine(appDataPath, "loginInfo.json");

            if (File.Exists(jsonFilePath))
            {
                File.Delete(jsonFilePath);
            }
        }
        private void ProfileIcon_Click(object sender, MouseButtonEventArgs e)
        {
            // Create an instance of ProfileWindow and show it
            _friendService = new FriendService(_friendRepo, _gamerRepo);

            ProfileWindow profileWindow = new ProfileWindow(_user, _friendService);
            profileWindow.Show();
            this.Hide();
            this.Close();

        }
        private void GamesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Ensure the sender is a ListBox
            if (sender is ListBox listBox)
            {
                // Get the selected game
                var selectedGame = listBox.SelectedItem as Game;

                // Check if a game is selected
                if (selectedGame != null)
                {
                    // Access the game's properties
                    InitializeGamerData(selectedGame, _user);

                }
            }
        }
        private void Home_Click(object sender, MouseButtonEventArgs e)
        {
            CustomerWindow cus = new CustomerWindow(_user);
            cus.Show();
            this.Hide();
            this.Close();
        }
        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                searchGame_button(sender, e);
            }
        }
        private void searchGame_button(object sender, RoutedEventArgs e)
        {
            var CurrentWin = _user;
            string Search_input = SearchTextBox.Text.Trim().ToLower();
            if (Search_input == "search name game")
            {
                Search_input = string.Empty;
            }
            SearchWindow search = new SearchWindow(CurrentWin, Search_input, null, null);
            search.Show();
            this.Hide();
            this.Close();
        }
        private void searchButton_Click(object sendedr, MouseButtonEventArgs e)
        {
            var CurrentUser = _user;
            SearchWindow serchwindow = new SearchWindow(CurrentUser, null, null, null);
            serchwindow.Show();
            this.Hide();
            this.Close();
        }
        private void OnTagClick(object sender, MouseButtonEventArgs e)
        {
            List<string> selectedGenre = new List<string>();
            var tagControl = sender as FLauncher.CC.tags;
            if (tagControl != null)
            {
                var genre = tagControl.DataContext as Genre; // Genre là lớp dữ liệu chứa TypeOfGenre
                if (genre != null)
                {
                    selectedGenre.Add(genre.TypeOfGenre); // Lấy TypeOfGenre


                    // Mở SearchWindow và truyền giá trị TypeOfGenre vào
                    SearchWindow searchWindow = new SearchWindow(_user, null, selectedGenre, null);
                    searchWindow.Show();
                    this.Close();
                }
            }
        }

    }




}

