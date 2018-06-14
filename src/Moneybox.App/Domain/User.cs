using System;

namespace Moneybox.App
{
    public class User
    {
        public User(string name, string email, Guid? id = null)
        {
            this.Id = id ?? Guid.NewGuid();
            this.Name = name;
            this.Email = email;
        }

        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public string Email { get; private set; }
    }
}
