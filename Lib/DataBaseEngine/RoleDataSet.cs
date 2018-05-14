namespace Big3.Hitbase.DataBaseEngine {
    
    
    public partial class RoleDataSet {
    }
}

namespace Big3.Hitbase.DataBaseEngine.RoleDataSetTableAdapters
{
    public partial class RoleTableAdapter
    {
        public RoleTableAdapter(DataBase db)
        {
            Connection = db.Connection;
        }
        
        public RoleDataSet.RoleRow GetRoleById(int id)
        {
            RoleDataSet roleDataset = new RoleDataSet();

            int numberOfRoles = FillById(roleDataset.Role, id);

            if (numberOfRoles > 0)
                return roleDataset.Role[0];
            else
                return null;
        }

        public RoleDataSet.RoleRow GetRoleByName(string role, bool createIfNotFound)
        {
            RoleDataSet roleDataset = new RoleDataSet();

            int numberOfRoles = FillByName(roleDataset.Role, role);

            if (numberOfRoles > 0)
            {
                return roleDataset.Role[0];
            }
            else
            {
                if (createIfNotFound && !string.IsNullOrEmpty(role))
                {
                    RoleDataSet.RoleRow roleRow = roleDataset.Role.NewRoleRow();
                    roleRow.Name = role;
                    roleDataset.Role.AddRoleRow(roleRow);
                    Update(roleRow);
                    //Updating, um ID zu erhalten
                    roleDataset.Role.Clear();
                    numberOfRoles = FillByName(roleDataset.Role, role);
                    if (numberOfRoles > 0)
                        return roleDataset.Role[0];
                }

                return null;
            }

        }
    }
}
