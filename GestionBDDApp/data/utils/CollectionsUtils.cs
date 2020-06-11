using System;
using System.Collections.Generic;

namespace GestionBDDApp.data.utils
{
    /// <summary>
    /// Classe utilitaire pour manipuler des collections
    /// </summary>
    public class CollectionsUtils
    {
        /// <summary>
        /// Retourne la valeur correspondant à la clé <paramref name="Key"/> dans le dictionnaire <paramref name="Dictionary"/>,
        /// si aucune valeur n'est trouvé pour cette clé dans le dictionnaire, une valeur par défaut fournis par le
        /// <paramref name="DefaultValueProvider"/> est placé dans le dictionnaire avec cette clé et est renvoyé. 
        /// </summary>
        /// <param name="Dictionary">Un dictionnaire non null</param>
        /// <param name="Key">La clé à laquelle chercher</param>
        /// <param name="DefaultValueProvider">Un fournisseur de valeur par défaut</param>
        /// <typeparam name="TKey">Le type des clés du dictionnaire</typeparam>
        /// <typeparam name="TValue">Le type des valeurs du dictionnaire</typeparam>
        /// <returns></returns>
        public static TValue GetOrCreate<TKey, TValue>(Dictionary<TKey, TValue> Dictionary, TKey Key, Func<TValue> DefaultValueProvider)
        {
            if (! Dictionary.TryGetValue(Key, out var Value))
            {
                var DefaultValue = DefaultValueProvider.Invoke();
                Dictionary.Add(Key, DefaultValue);
                return DefaultValue;
            }
            return Value;
        }
    }
}