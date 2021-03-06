﻿using Microsoft.Azure.Mobile.Analytics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using YallaParkingMobile.Model;
using YallaParkingMobile.Utility;
using System.Collections.ObjectModel;
using ZXing.Net.Mobile.Forms;
using System.Net;
using Plugin.MediaManager;
using Plugin.SimpleAudioPlayer;
using SkiaSharp.Views.Forms;

namespace YallaParkingMobile
{
    public partial class BookParking : ContentPage
    {
        void Handle_Tapped(object sender, System.EventArgs e)
        {
            throw new NotImplementedException();
        }

        private TableSection oneCar;
        private TableSection twoCars;
        private TableSection newCar;

        private TableSection oneCard;
        private TableSection twoCards;
        private TableSection newCard;

        private Frame parkNowTotalTime;

        private Frame parkLaterTotalTime;

        private TextCell parkingFreeMinutes;

        void Handle_Appearing(object sender, System.EventArgs e)
        {
            throw new NotImplementedException();
        }

        public BookParking()
        {
            InitializeComponent();
        }

        public BookParking(BookParkingModel model)
        {
            this.Model = model;

            InitializeComponent();

            this.parkNowTotalTime = ParkNowTotalTime;
            this.parkLaterTotalTime = ParkLaterTotalTime;
            this.parkingFreeMinutes = ParkingFreeMinutes;

            if (string.IsNullOrWhiteSpace(this.Model.FreeMinutes))
            {
                Order.Remove(parkingFreeMinutes);
            }

            if (this.Model.IsParkLater)
            {
                Stack.Children.Remove(parkNowTotalTime);
                Order.Remove(ParkNowDiscount);
            }
            else
            {
                Order.Remove(ParkLaterDiscount);
                Order.Remove(ParkLaterTotal);
                Order.Remove(ParkLaterHours);
                Stack.Children.Remove((ParkLaterTotalTime));
            }

            this.oneCar = OneCar;
            this.twoCars = TwoCars;
            this.newCar = AddNewCar;

            this.oneCard = OneCard;
            this.twoCards = TwoCards;
            this.newCard = AddNewCard;

            Analytics.TrackEvent("Viewing Booking Page");

            Appearing += BookParking_Appearing;

            if (this.Model.AllDay)
            {
                if (Order.Contains(PriceHour))
                {
                    Order.Remove(PriceHour);
                }
            }
            else
            {
                if (Order.Contains(PriceDay))
                {
                    Order.Remove(PriceDay);
                }
            }

        }

        public BookParkingModel Model
        {
            get
            {
                return (BookParkingModel)this.BindingContext;
            }
            set
            {
                this.BindingContext = value;
            }
        }

        async void BookParking_Appearing(object sender, EventArgs e)
        {
            base.OnAppearing();
            this.InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, " ");

            this.parkNowTotalTime = ParkNowTotalTime;
            this.parkLaterTotalTime = ParkLaterTotalTime;
            this.parkingFreeMinutes = ParkingFreeMinutes;

            if (string.IsNullOrWhiteSpace(this.Model.FreeMinutes)) {
                Order.Remove(parkingFreeMinutes);
            }

            if (this.Model.IsParkLater) {
                Stack.Children.Remove(parkNowTotalTime);
                Order.Remove(ParkNowDiscount);
            } else {
                Order.Remove(ParkLaterDiscount);
                Order.Remove(ParkLaterTotal);
                Order.Remove(ParkLaterHours);
                Stack.Children.Remove((ParkLaterTotalTime));
            }

            if (this.Model.AllDay) {
                if (Order.Contains(PriceHour)) {
                    Order.Remove(PriceHour);
                }
            } else {
                if (Order.Contains(PriceDay)) {
                    Order.Remove(PriceDay);
                }
            }

            var userCars = await ServiceUtility.GetUserCars();

            if (userCars != null && userCars.Any())
            {
                if (userCars.Count == 1)
                {
                    this.Model.TotalCar = 1;
                }
                else if (userCars.Count == 2)
                {
                    this.Model.TotalCar = 2;
                }
                else
                {
                    this.Model.TotalCar = userCars.Count();
                }


            }

            TableView.Remove(TwoCars);
            TableView.Remove(OneCar);
            TableView.Remove(AddNewCar);

            if (this.Model.TotalCar < 1) {
                TableView.Add(AddNewCar);
            } else if (this.Model.TotalCar == 1) {
                TableView.Add(oneCar);
            } else if (this.Model.TotalCar >= 2) {
                TableView.Add(twoCars);
            }

            this.Model.UserCars = new ObservableCollection<UserCarModel>(userCars);

            var userCards = await ServiceUtility.GetUserCards();

            if (userCards != null && userCards.Any())
            {
                if (userCards.Count == 1)
                {
                    this.Model.TotalCard = 1;
                }
                else if (userCards.Count == 2)
                {
                    this.Model.TotalCard = 2;
                }
                else
                {
                    this.Model.TotalCard = userCards.Count();
                }
            }

            TableView.Remove(TwoCards);
            TableView.Remove(OneCard);
            TableView.Remove(AddNewCard);

            if (this.Model.TotalCard < 1) {
                TableView.Add(newCard);
            } else if (this.Model.TotalCard == 1) {
                TableView.Add(oneCard);
            } else if (this.Model.TotalCard >= 2) {
                TableView.Add(twoCards);
            }

            this.Model.UserCards = new ObservableCollection<UserCardModel>(userCards);

            await this.Model.SetBookingDefaults();
        }

        private async void ApplyCodeButton_Clicked(object sender, EventArgs e)
        {
            var discountCode = DiscountCode.Text;

            if (!string.IsNullOrWhiteSpace(discountCode))
            {
                var discountResponse = await ServiceUtility.DiscountCode(discountCode);

                if (discountResponse.Discount.HasValue)
                {
                    this.Model.Property.Discount = this.Model.Property.TotalPrice * (discountResponse.Discount.Value / 100);
                    this.Model.Discount = discountResponse.Discount.Value;
                    this.Model.DiscountCode = discountResponse.DiscountCode;
                    this.ApplyCodeButton.IsVisible = false;
                    this.DiscountCode.IsEnabled = false;
                    Order.Remove(DiscountCode);
                    Order.Remove(ApplyCodeCell);

                    await DisplayAlert("Success!", "Your promotional code has been successfully added.", "Ok");
                }
                else
                {
                    await DisplayAlert("Invalid Promotional Code", "Invalid or expired promotional code provided.", "Ok");
                }
            }
            else
            {
                await DisplayAlert("No Promotional Code", "No promotional code provided.", "Ok");
            }
        }


        async void AddNewCarButton_Clicked(object sender, EventArgs e)
        {
            var updateCarDetails = new UpdateCarDetails();
            var userCar = new UserCarModel();
            updateCarDetails.BindingContext = userCar;
            await Navigation.PushAsync(updateCarDetails);
        }

        async void AddNewCardButton_Clicked(object sender, EventArgs e)
        {
            var updateCardDetails = new UpdateCardDetails();
            var userCard = new UserCardModel();
            updateCardDetails.BindingContext = userCard;
            await Navigation.PushAsync(updateCardDetails);
        }

        async void Book_Clicked(object sender, System.EventArgs e)
        {
            if (Model.SelectedUserCar == null || Model.SelectedUserCard == null) {
                await DisplayAlert("Booking Error", "Please select a car and a card to proceed with your booking", "Ok");
            } else {
                //BookButton.IsEnabled = false;

                if (Model.ParkingNow) {
                    var scanPage = new ZXingScannerPage();
                    bool scanFinished = false;

                    scanPage.OnScanResult += (result) => {
                        // Stop scanning
                        scanPage.IsScanning = false;

                        // Pop the page and show the result
                        Device.BeginInvokeOnMainThread(async () => {
                            if (!scanFinished) {
                                scanFinished = true;

                                if (result.Text == Model.Property.PropertyId.ToString()) {
                                    var bookingResponse = await Model.BookParking();

                                    if (bookingResponse.IsSuccessStatusCode) {
                                        Model.BookingNumber = await bookingResponse.Content.ReadAsStringAsync();
                                        Model.BookingNumber = string.Format("#{0}", Model.BookingNumber).Replace("\"", "");

                                        var entry = await ServiceUtility.Entry(Model.Property.PropertyId, Model.Property.StartDate);
                                        //BookButton.IsEnabled = true;

                                        if (entry) {
                                            var bookingConfirmation = new BookingConfirmation(this.Model);
                                            bookingConfirmation.BindingContext = Model.BookingNumber;
                                            await Navigation.PushAsync(bookingConfirmation);
                                        } else {

                                            await DisplayAlert("Entry Error", "There was an error entering the parking space, please try again", "Ok");
                                        }
                                    } else if (bookingResponse.StatusCode == HttpStatusCode.Conflict) {

                                        await DisplayAlert("Booking Exists Error", "There is already a booking exists for this property", "Ok");
                                    } else if (bookingResponse.StatusCode == HttpStatusCode.NotFound) {

                                        await DisplayAlert("Booking Error", "If this is your first booking, please select a car and a card to proceed with your booking", "Ok");
                                    } else {

                                        await DisplayAlert("Booking Error", "There was an error confirming your booking, please try again", "Ok");
                                        await Navigation.PopAsync();
                                    }

                                } else {

                                    await DisplayAlert("Invalid Scan", "The QR code scanned does not match the property for this booking", "Ok");
                                    await Navigation.PopAsync();
                                }
                            }
                        });
                    };

                    await Navigation.PushAsync(scanPage);

                } else {
                    var bookingResponse = await Model.BookParking();
                    //BookButton.IsEnabled = true;

                    if (bookingResponse.IsSuccessStatusCode) {
                        Model.BookingNumber = await bookingResponse.Content.ReadAsStringAsync();
                        Model.BookingNumber = string.Format("#{0}", Model.BookingNumber).Replace("\"", "");

                        var bookingConfirmation = new BookingConfirmation(this.Model);
                        bookingConfirmation.BindingContext = Model.BookingNumber;
                        await Navigation.PushAsync(bookingConfirmation);

                    } else if (bookingResponse.StatusCode == HttpStatusCode.Conflict) {
                        await DisplayAlert("Booking Exists Error", "There is already an ongoing booking for this property", "Ok");

                    } else if (bookingResponse.StatusCode == HttpStatusCode.NotFound) {

                        await DisplayAlert("Booking Error", "If this is your first booking, please select a car and a card to proceed with yor booking", "Ok");
                    } else {
                        await DisplayAlert("Booking Error", "There was an error confirming your booking, please try again", "Ok");

                    }
                }
            }
        }
    }
}
