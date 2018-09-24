﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Chiota.Models;
using Chiota.ViewModels.Classes;
using Xamarin.Forms;

namespace Chiota.ViewModels.Messenger
{
    public class ChatsViewModel : BaseViewModel
    {
        #region Attributes

        private static List<Chat> _chatList;

        #endregion

        #region Properties

        public List<Chat> ChatList
        {
            get => _chatList;
            set
            {
                _chatList = value;
                OnPropertyChanged(nameof(ChatList));
            }
        }

        #endregion

        #region Init

        public override void Init(object data = null)
        {
            UpdateView();

            base.Init(data);
        }

        #endregion

        #region Methods

        #region UpdateView

        private void UpdateView()
        {
            var tmp = new List<Chat>
            {
                new Chat()
                {
                    Name = "David",
                    LastMessage = "Hi",
                    LastMessageTime = DateTime.Now.ToString("d", CultureInfo.CurrentCulture),
                    ImageSource = ImageSource.FromFile("account.png")
                },
                new Chat()
                {
                    Name = "Sebastian",
                    LastMessage = "Great",
                    LastMessageTime = DateTime.Now.ToString("d", CultureInfo.CurrentCulture),
                    ImageSource = ImageSource.FromFile("account.png")
                }
            };

            //Add all chats of the user to the ui.
            ChatList = tmp;
        }

        #endregion

        #endregion
    }
}
