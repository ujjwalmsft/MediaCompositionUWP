//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using Windows.Media.Editing;
using Windows.Media.Core;
using SDKTemplate;
using System.Linq;
using Windows.Media.Effects;
using Windows.Media;
using Windows.Foundation.Collections;

namespace MediaEditingSample
{
    public sealed partial class Scenario2_AppendClips : Page
    {
        private MainPage rootPage;
        private MediaComposition composition;
        private StorageFile firstVideoFile;
        private StorageFile secondVideoFile;
        private MediaStreamSource mediaStreamSource;

        public Scenario2_AppendClips()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            rootPage = MainPage.Current;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            mediaElement.Source = null;
            mediaStreamSource = null;
            base.OnNavigatedFrom(e);
        }

        private async void ChooseFirstVideo_Click(object sender, RoutedEventArgs e)
        {
            // Get first file
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.VideosLibrary;
            picker.FileTypeFilter.Add(".mp4");
            firstVideoFile = await picker.PickSingleFileAsync();
            if (firstVideoFile == null)
            {
                rootPage.NotifyUser("File picking cancelled", NotifyType.ErrorMessage);
                return;
            }

            mediaElement.SetSource(await firstVideoFile.OpenReadAsync(), firstVideoFile.ContentType);
            chooseSecondFile.IsEnabled = true;
        }

        private async void ChooseSecondVideo_Click(object sender, RoutedEventArgs e)
        {
            // Get second file
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.VideosLibrary;
            picker.FileTypeFilter.Add(".mp4");
            secondVideoFile = await picker.PickSingleFileAsync();
            if (secondVideoFile == null)
            {
                rootPage.NotifyUser("File picking cancelled", NotifyType.ErrorMessage);
                return;
            }

            mediaElement.SetSource(await secondVideoFile.OpenReadAsync(), secondVideoFile.ContentType);
            appendFiles.IsEnabled = true;
        }

        private async void AppendVideos_Click(object sender, RoutedEventArgs e)
        {
            // Combine two video files together into one
            var firstClip = await MediaClip.CreateFromFileAsync(firstVideoFile);
            var secondClip = await MediaClip.CreateFromFileAsync(secondVideoFile);

            composition = new MediaComposition();
            composition.Clips.Add(firstClip);
            //VideoStabilizationEffectDefinition videoEffect = new VideoStabilizationEffectDefinition();
            var effectDefinition = new VideoEffectDefinition("muveeVideoEffectsComponent.ExampleVideoEffect"
                //,new PropertySet() { { "FadeValue", .5 } }
                );
            firstClip.VideoEffectDefinitions.Add(effectDefinition);
            composition.Clips.Add(secondClip);

            //AddVideoEffect(composition);

            // Render to MediaElement.
            mediaElement.Position = TimeSpan.Zero;
            mediaStreamSource = composition.GeneratePreviewMediaStreamSource((int)mediaElement.ActualWidth, (int)mediaElement.ActualHeight);
            mediaElement.SetMediaStreamSource(mediaStreamSource);
            rootPage.NotifyUser("Clips appended", NotifyType.StatusMessage);
        }

        //private void AddVideoEffect(MediaComposition composition)
        //{
        //    var currentClip = composition.Clips.FirstOrDefault(
        //        mc => mc.StartTimeInComposition <= mediaElement.MediaPlayer.PlaybackSession.Position &&
        //        mc.EndTimeInComposition >= mediaElement.MediaPlayer.PlaybackSession.Position);

        //    VideoStabilizationEffectDefinition videoEffect = new VideoStabilizationEffectDefinition();
        //    currentClip.VideoEffectDefinitions.Add(videoEffect);
        //}
    }
}
