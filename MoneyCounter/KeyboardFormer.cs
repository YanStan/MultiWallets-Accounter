using MoneyCounter.Repositories;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types.ReplyMarkups;

namespace MoneyCounter
{
    class KeyboardFormer
    {
        public InlineKeyboardMarkup FormStartCmdTextKeyboardForNoAdmin()
        {
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("🧵 Внести денежный перевод", "Внести перевод")
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("📈 Анализировать данные", "Анализировать данные")
                    },
            });
            return keyboard;
        }

        public InlineKeyboardMarkup FormStartCmdTextKeyboardForAdmin()
        {
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("🧵 Внести денежный перевод", "Внести перевод")
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("📈 Анализировать данные", "Анализировать данные")
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("🎩 Админ-панель", "Админ-панель")
                    },
            });
            return keyboard;
        }


        public ReplyKeyboardMarkup FormAdminUsernames()
        {
            UserRepository repos = new UserRepository();
            var adminUserNames = repos.GetAdminUsernames();
            var keyboardMarkup = new ReplyKeyboardMarkup();
            keyboardMarkup.Keyboard = adminUserNames.Select(x => new KeyboardButton(x)).ToList()
                .Partition(3).ToArray();
            keyboardMarkup.ResizeKeyboard = true;
            return keyboardMarkup;
        }

        public ReplyKeyboardMarkup FormUsernames(bool IsAdmins)
        {
            UserRepository repos = new UserRepository();
            List<string> userNames;
            if (IsAdmins)
                userNames = repos.GetAdminUsernames();       
            else
                userNames = repos.GetNonAdminUsernames();
            var keyboardMarkup = new ReplyKeyboardMarkup();
            keyboardMarkup.Keyboard = userNames.Select(x => new KeyboardButton(x)).ToList()
                .Partition(3).ToArray();
            keyboardMarkup.ResizeKeyboard = true;
            return keyboardMarkup;
        }

        public ReplyKeyboardMarkup FormUsernamesFromWhiteList()
        {
            UserRepository repos = new UserRepository();
            var userNames = repos.GetUsernamesFromWhiteList();
            var keyboardMarkup = new ReplyKeyboardMarkup();
            keyboardMarkup.Keyboard = userNames.Select(x => new KeyboardButton(x)).ToList()
                .Partition(3).ToArray();
            keyboardMarkup.ResizeKeyboard = true;
            return keyboardMarkup;
        }

        public InlineKeyboardMarkup FormAdminPanelKeyboard()
        {
            var keyboard = new InlineKeyboardMarkup(new[]
{
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("🧵 Внести перевод между основателями", "Внести основ. перевод")
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("📈 Анализировать переводы основателей", "Анализировать основ. данные")
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("☂️ Управление пользователями", "Управление пользователями")
                    },
            });
            return keyboard;
        }

        public ReplyKeyboardMarkup FormUserManagement()
        {
            var keyboardMarkup = new ReplyKeyboardMarkup();
            keyboardMarkup.Keyboard = new KeyboardButton[][]
            {
                new KeyboardButton[]
                    {
                        new KeyboardButton("✴️ Добавить администратора"),
                        new KeyboardButton("📴 Удалить администратора"),

                    },
                new KeyboardButton[]
                    {
                        new KeyboardButton("💟 Добавить пользователя"),
                        new KeyboardButton("♒️ Удалить пользователя"),
                    },
            };
            keyboardMarkup.ResizeKeyboard = true;
            keyboardMarkup.OneTimeKeyboard = true;
            return keyboardMarkup;
        }
        public ReplyKeyboardMarkup FormAnalyseDataKeyboard()
        {
            var keyboardMarkup = new ReplyKeyboardMarkup();
            keyboardMarkup.Keyboard = new KeyboardButton[][]
            {
                new KeyboardButton[]
                    {
                        new KeyboardButton("💲 Доходы и расходы"),
                        new KeyboardButton("💼 Состояние кошельков")
                    },
                new KeyboardButton[]
                    {
                        new KeyboardButton("📈 Доходные счета"),
                        new KeyboardButton("🎞 История переводов")
                    }
            };
            keyboardMarkup.ResizeKeyboard = true;
            return keyboardMarkup;
        }
        public ReplyKeyboardMarkup FormForWalletHistoryType()
        {
            var keyboardMarkup = new ReplyKeyboardMarkup();
            keyboardMarkup.Keyboard = new KeyboardButton[][]
            {
                new KeyboardButton[]
                    {
                        new KeyboardButton("🔗Один кошелек"),
                        new KeyboardButton("Все кошельки 🖇")
                    },
            };
            keyboardMarkup.ResizeKeyboard = true;
            keyboardMarkup.OneTimeKeyboard = true;
            return keyboardMarkup;
        }

            public ReplyKeyboardMarkup FormTransactAllMoneyKeyboard(int moneySum)
        {
            var keyboardMarkup = new ReplyKeyboardMarkup();
            keyboardMarkup.Keyboard = new KeyboardButton[][]
            {
                new KeyboardButton[]
                {
                    new KeyboardButton($"Вся сумма ({moneySum} грн)"),
                }
            };
            keyboardMarkup.ResizeKeyboard = true;
            return keyboardMarkup;
        }

        public ReplyKeyboardMarkup FormTransactAllMoneyOrMultiplyKeyboard(int moneySum, int inputMoneySum)
        {
            var keyboardMarkup = new ReplyKeyboardMarkup();
            keyboardMarkup.Keyboard = new KeyboardButton[][]
            {
                new KeyboardButton[]
                {
                    new KeyboardButton($"Вся сумма ({moneySum} грн)"),
                },
                new KeyboardButton[]
                {
                    new KeyboardButton($"Подтвердить снятие заработанных {inputMoneySum} грн"),
                }
            };
            keyboardMarkup.ResizeKeyboard = true;
            return keyboardMarkup;
        }

        public ReplyKeyboardMarkup FormAreYouSureItsFoundersWallet(string walletName)
        {
            var keyboardMarkup = new ReplyKeyboardMarkup();
            keyboardMarkup.Keyboard = new KeyboardButton[][]
            {
                new KeyboardButton[]
                {
                    new KeyboardButton($"Да, перевести с {walletName}"),
                },
                new KeyboardButton[]
                {
                    new KeyboardButton($"Нет, я ошибся вводом"),
                }
            };
            keyboardMarkup.ResizeKeyboard = true;
            return keyboardMarkup;
        }

        public ReplyKeyboardMarkup FormYesOrNoConfirmDeletion(int transactionId)
        {
            var keyboardMarkup = new ReplyKeyboardMarkup();
            keyboardMarkup.Keyboard = new KeyboardButton[][]
            {
                new KeyboardButton[]
                {
                    new KeyboardButton($"✅ Удалить перевод №{transactionId}"),
                },
                new KeyboardButton[]
                {
                    new KeyboardButton($"❌ Отказать в удалении п. №{transactionId}"),
                }
            };
            keyboardMarkup.ResizeKeyboard = true;
            keyboardMarkup.OneTimeKeyboard = true;
            return keyboardMarkup;
        }

        public ReplyKeyboardMarkup FormTransactionTypeKeyboard()
        {
            var keyboardMarkup = new ReplyKeyboardMarkup();
            keyboardMarkup.Keyboard = new KeyboardButton[][]
            {
                new KeyboardButton[]
                    {
                        new KeyboardButton("👨‍✈️🔛 И начальный, и конечный"),
                    },
                new KeyboardButton[]
                    {
                        new KeyboardButton("👨‍✈️🔜 Начальный"),
                    },
                new KeyboardButton[]
                    {
                        new KeyboardButton("👨‍💼⏭ Конечный"),
                    },
                new KeyboardButton[]
                    {
                        new KeyboardButton("👨‍💼🔄 Не начальный и не конечный"),
                    },
                new KeyboardButton[]
                    {
                        new KeyboardButton("👨‍💼🔙 Реверсивный"),
                    },
                new KeyboardButton[]
                    {
                        new KeyboardButton("🟢❇️  Чистый доход ❇️ 🟢"),
                    },
            };
            keyboardMarkup.ResizeKeyboard = true;
            return keyboardMarkup;
        }

        public ReplyKeyboardMarkup FormMaybeYouHaveMentionedKeyboard(string inputWallet, string usedWallet)
        {
            var keyboardMarkup = new ReplyKeyboardMarkup();
            keyboardMarkup.Keyboard = new KeyboardButton[][]
            {
                new KeyboardButton[]
                    {
                        new KeyboardButton(usedWallet),
                    },
                new KeyboardButton[]
                    {
                        new KeyboardButton(inputWallet),
                    },
            };
            keyboardMarkup.ResizeKeyboard = true;
            return keyboardMarkup;
        }
        public ReplyKeyboardMarkup FormCategoriesTextKeyboard(string entityTypeUpper)
        {
            var repositoryFactory = new FinanceEntityRepositoryFactory();
            var repos = repositoryFactory.GetRepositoryInstanceFromItsUpperName(entityTypeUpper);
            string[] uniqueCategoryNames = repos.GetEntityCategoriesNames();
            var keyboardMarkup = new ReplyKeyboardMarkup();
            keyboardMarkup.Keyboard = uniqueCategoryNames.Select(x => new KeyboardButton(x)).ToList().Partition(2).ToArray().Concat(new[]
                {
                    new KeyboardButton[]
                    {
                        new KeyboardButton("⚙️ Добавить категорию ⚙️")
                    },
                    new KeyboardButton[]
                    {
                        new KeyboardButton("⚙️ Переименовать к."),
                        new KeyboardButton("Удалить к. ⚙️"),
                    }
                });
            keyboardMarkup.ResizeKeyboard = true;
            return keyboardMarkup;
        }

        public ReplyKeyboardMarkup FormCategoriesTextKeyboardForChanging(string[] categoriesNames)
        {
            var keyboardMarkup = new ReplyKeyboardMarkup();
            keyboardMarkup.Keyboard = categoriesNames.Select(x => new KeyboardButton(x)).ToList()
                .Partition(3).ToArray();
            keyboardMarkup.ResizeKeyboard = true;
            return keyboardMarkup;
        }
        public ReplyKeyboardMarkup FormSubCategoriesFromList(string[] subcategoryNames)
        {
            var keyboardMarkup = new ReplyKeyboardMarkup();
            keyboardMarkup.Keyboard = subcategoryNames.Select((x, i) => new KeyboardButton(x)).ToList()
                .Partition(3).Concat(new[]
                {
                    new KeyboardButton[]
                    {
                        new KeyboardButton("⚙️ Добавить субкатегорию ⚙️")
                    },
                    new KeyboardButton[]
                    {
                        new KeyboardButton("⚙️ Переименовать с."),
                        new KeyboardButton("Удалить с. ⚙️"),
                    }
                }).ToArray();
            keyboardMarkup.ResizeKeyboard = true;
            return keyboardMarkup;
        }

        public ReplyKeyboardMarkup FormSubCategoriesFromListForChanging(List<string> subcategoryNames)
        {
            var keyboardMarkup = new ReplyKeyboardMarkup();
            keyboardMarkup.Keyboard = subcategoryNames.Select(x => new KeyboardButton(x)).ToList()
            .Partition(3).ToArray();
            keyboardMarkup.ResizeKeyboard = true;
            return keyboardMarkup;
        }

        public ReplyKeyboardMarkup FormUsedWalletsKeyboard(string entityNameUpper)
        {
            TransactionRepository reposofTrans = new TransactionRepository();
            var walletNames = new List<string> { };
            switch (entityNameUpper)
            {
                case "TRANSACTION":
                    walletNames = reposofTrans.GetUsedTransactionWallets();
                    break;
                case "FOUNDERSTRANSACTION":
                    walletNames = reposofTrans.GetAllUsedFoundersWallets();
                    break;
            }
            var keyboardMarkup = new ReplyKeyboardMarkup();
            keyboardMarkup.Keyboard = walletNames.Select(x => new KeyboardButton(x)).ToList()
            .Partition(3).ToArray();
            keyboardMarkup.ResizeKeyboard = true;
            return keyboardMarkup;
        }

        public ReplyKeyboardMarkup FormTransactionConfidenceAnswers()
        {
            var keyboardMarkup = new ReplyKeyboardMarkup();
            keyboardMarkup.Keyboard = new KeyboardButton[][]
            {
                new KeyboardButton[]
                    {
                        new KeyboardButton("Да, сохранить данные"),
                    },
                new KeyboardButton[]
                    {
                        new KeyboardButton("Нет, ввести другие данные"),
                    },
            };
            keyboardMarkup.OneTimeKeyboard = true;
            keyboardMarkup.ResizeKeyboard = true;
            return keyboardMarkup;
        }

        public ReplyKeyboardMarkup FormTransactionsManipulating()
        {
            var keyboardMarkup = new ReplyKeyboardMarkup();
            keyboardMarkup.Keyboard = new KeyboardButton[][]
            {
                new KeyboardButton[]
                    {
                        new KeyboardButton("Удалить трансакцию по номеру"),
                    },
                new KeyboardButton[]
                    {
                        new KeyboardButton("Изменить сумму перевода по номеру"),
                    },
            };
            keyboardMarkup.OneTimeKeyboard = true;
            keyboardMarkup.ResizeKeyboard = true;
            return keyboardMarkup;
        }

        public ReplyKeyboardMarkup FormDaysForAnalysisKeyboard()
        {
            var keyboardMarkup = new ReplyKeyboardMarkup();
            keyboardMarkup.Keyboard = new KeyboardButton[][]
                {
                    new []
                    {
                        new KeyboardButton("За последний день"),
                        new KeyboardButton("За последние 2 дня")
                    },
                    new []
                    {
                        new KeyboardButton("За последний месяц"),
                        new KeyboardButton("За всё время")
                    },
                    new []
                    {
                        new KeyboardButton("⌨️ За последние N дней"),
                        new KeyboardButton("От времени до времени 📐")
                    }
                };
            keyboardMarkup.OneTimeKeyboard = false;
            keyboardMarkup.ResizeKeyboard = true;
            return keyboardMarkup;
        }
    }
}

