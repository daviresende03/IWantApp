namespace IWantApp.Endpoints
{
    public static class ProblemDetailsExtensions
    {
        //Cria o retorno de mensagens da descrição do erro, agrupado pelo campo com erro (key).
        public static Dictionary<string,string[]> ConvertToProblemDetails(this IReadOnlyCollection<Notification> notifications)
        {
            return notifications
                .GroupBy(g => g.Key)
                .ToDictionary(g => g.Key, g => g.Select(x => x.Message)
                .ToArray());
        }

        //Erros Identity
        public static Dictionary<string, string[]> ConvertToProblemDetails(this IEnumerable<IdentityError> error)
        {
            var dictionary = new Dictionary<string, string[]>();
            dictionary.Add("Error", error.Select(x => x.Description).ToArray());


            return dictionary;
        }
    }
}
