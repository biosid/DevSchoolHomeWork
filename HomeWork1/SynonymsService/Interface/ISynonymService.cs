using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynonymsService.Interface
{
    public interface ISynonymService
    {

        /// <summary>
        /// Получить список синонимов к слову
        /// </summary>
        /// <param name="type">тип части речи</param>
        /// <param name="word">исходное слово</param>
        /// <returns></returns>
        Task<List<string>> GetSynonymsAsync(SynonymType type, string word);

        /// <summary>
        /// Получить какой нибудь случайный синоним к слову
        /// </summary>
        /// <param name="type">тип части речи</param>
        /// <param name="word">исходное слово</param>
        /// <returns></returns>
        Task<string> GetRandomSynonymAsync(SynonymType type, string word);

    }
}
