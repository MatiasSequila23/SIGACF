using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Contracts;
using DAL.Tools;
using Domain;

namespace DAL.Repositories.SqlServer
{
    internal class UserRepository : IGenericRepository<User>
    {
        List<User> user = new List<User>();

        #region Statements
        private string InsertStatement
        {
            get => "INSERT INTO [dbo].[Users] (LoginName, Password, FirstName, LastName, Position, Email) VALUES (@LoginName, @Password, @FirstName, @LastName, @Podition, @email)";
        }

        private string UpdateStatement
        {
            get => "UPDATE [dbo].[Users] SET (LoginName, Password, FirstName, LastName, Position, Email) WHERE UserId = @UserId";
        }

        private string DeleteStatement
        {
            get => "DELETE FROM [dbo].[Users] WHERE UserId = @UserId";
        }

        private string SelectOneStatement
        {
            get => "SELECT UserId, LoginName, Password, FirstName, LastName, Position, Email FROM [dbo].[Users] WHERE UserId = @UserId";
        }

        private string SelectAllStatement
        {
            get => "SELECT UserId, LoginName, Password, FirstName, LastName, Position, Email FROM [dbo].[Users]";
        }
        #endregion

        public void Delete(Guid UserId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> GetAll()
        {
            throw new NotImplementedException();
        }

        public User GetOne(Guid UserId)
        {
            User user = default;

            using (var dr = SqlHelper.ExecuteReader(SelectOneStatement, System.Data.CommandType.Text, null, null))

            {
                if (dr.Read())
                {
                    object[] values = new object[dr.FieldCount];
                    dr.GetValues(values);
                }

                return user;
            }
        }

        public void Insert(User Object)
        {
            //SqlHelper.ExecuteNonQuery(InsertStatement, System.Data.CommandType.Text,
            //    new SqlParameter[] {    new SqlParameter("@LoginName", object.LoginName),
            //                            new SqlParameter("@Password", object.Password),
            //                            new SqlParameter("@FirstName", object.FirstName),
            //                            new SqlParameter("@LastName", object.LastName),
            //                            new SqlParameter("@Position", object.Position),
            //                            new SqlParameter("@Email", object.Email) 
            //    });
        
        }

       public void Update(Guid UserId, User Object)
       {
            throw new NotImplementedException();
       }
    }
}
