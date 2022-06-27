namespace IWantApp.Domain.Products
{
    public class Category : Entity
    {
        public string Name { get; private set; }
        public bool Active { get; private set; }
        
        public Category(string name,string createdBy,string editedBy )
        {
            Name = name;
            Active = true;
            CreatedBy = createdBy;
            CreatedOn = DateTime.Now;
            EditedBy = editedBy;
            EditedOn = DateTime.Now;

            Validate();
        }

        private void Validate()
        {
            var contract = new Contract<Category>()
                            .IsNotNullOrEmpty(Name, "Name", "Nome é um campo Obrigatório.")
                            .IsGreaterOrEqualsThan(Name, 2, "Name", "Nome deve conter no mínimo 2 caracteres.")
                            .IsNotNullOrEmpty(CreatedBy, "CreatedBy")
                            .IsNotNullOrEmpty(EditedBy, "EditedBy");
            AddNotifications(contract);
        }

        public void EditInfo(string name, bool active, string editedBy)
        {
            Active = active;
            Name = name;
            EditedBy = editedBy;
            EditedOn = DateTime.Now;

            Validate();
        }
    }
}
