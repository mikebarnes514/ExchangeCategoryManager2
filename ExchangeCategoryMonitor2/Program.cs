using ExchangeCategoryMonitor2.Categories;
using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ExchangeCategoryMonitor2
{
    class Program
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            ExchangeService service = new ExchangeService();
            FolderId calendarId = WellKnownFolderName.Calendar;

            service.Credentials = new WebCredentials("svc_efsadmin", "hB7FflVLtD", "MJSC");
            service.AutodiscoverUrl("barnesma@millerjohnson.com");
            service.ImpersonatedUserId = new ImpersonatedUserId(ConnectingIdType.SmtpAddress, "barnesma@millerjohnson.com");
            
            foreach(string username in ReadUsers())
            {
                try
                {
                    UpdateMasterFromLocal(service, username);
                    if (CheckForUpdatesToLocal(service, username))
                    {
                        Log.Info(String.Format("Local template category list for {0} was missing entries. Updating local template.", username));
                        UpdateLocalFromMaster(service, username);
                    }
                }
                catch(Exception ex)
                {
                    Log.Error("Failed to update master category list for user '" + username + ".", ex);
                }
            }            
        }

        static List<string> ReadUsers()
        {
            List<string> users = new List<string>();

            foreach (string f in Directory.GetFiles("templates", "*.xml", SearchOption.TopDirectoryOnly))
                users.Add(Path.GetFileNameWithoutExtension(f));

            return users;
        }

        static MasterCategoryList GetMasterList(ExchangeService service, string username)
        {
            return MasterCategoryList.Bind(service, String.Format("{0}@millerjohnson.com", username));
        }

        static MasterCategoryList GetLocalList(string username)
        {
            MasterCategoryList list = new MasterCategoryList();

            try
            {
                TextReader reader = new StreamReader(String.Format(".\\templates\\{0}.xml", username));
                XmlSerializer serializer = new XmlSerializer(typeof(MasterCategoryList));
                list = (MasterCategoryList)serializer.Deserialize(reader);
                reader.Close();
            }
            catch
            {

            }
            

            return list;
        }

        static void UpdateMasterFromLocal(ExchangeService service, string username)
        {
            var master = GetMasterList(service, username);
            var local = GetLocalList(username);
            bool needsUpdate = false;

            foreach (Category c in local.Categories)
            {
                try
                {
                    if (!master.Categories.Any(cat => cat.Name == c.Name))
                    {
                        Log.Warn(String.Format("Category '{0}' did not exist in Exchange. Adding to Master Category List.", c.Name));
                        master.Categories.Add(new Category(c.Name, c.Color, c.KeyboardShortcut));
                        needsUpdate = true;
                    }
                    else if (master.Categories.Single(cat => cat.Name == c.Name).Color != c.Color)
                    {
                        Log.Warn(String.Format("Setting category '{0}' to color {1}.", c.Name, c.Color.ToString()));
                        master.Categories.Single(cat => cat.Name == c.Name).Color = c.Color;
                        needsUpdate = true;
                    }
                }
                catch(Exception ex)
                {
                    Log.Error(String.Format("Failed to update master category list entry {0} for {1}", c.Name, username), ex);
                }
            }

            if (needsUpdate)
            {
                Log.Info(String.Format("Updating Master Category List for {0}.", username));
                try
                {
                    master.Update();
                }
                catch(Exception ex)
                {
                    Log.Error(String.Format("Failed to save Master Category List for {0}", username), ex);
                }
            }
        }

        static bool CheckForUpdatesToLocal(ExchangeService service, string username)
        {
            var master = GetMasterList(service, username);
            var local = GetLocalList(username);
            bool isUpToDate = true;

            foreach(Category c in master.Categories)
            {
                if(!local.Categories.Any(cat => cat.Name == c.Name))
                {
                    isUpToDate = false;
                    break;
                }
            }

            return !isUpToDate;
        }

        static void UpdateLocalFromMaster(ExchangeService service, string username)
        {
            FileStream file = null; 
            XmlSerializer writer = new XmlSerializer(typeof(MasterCategoryList));

            try
            {
                var list = MasterCategoryList.Bind(service, String.Format("{0}@millerjohnson.com", username));
                file = File.Create(String.Format(".\\templates\\{0}.xml", username));
                writer.Serialize(file, list);
            }
            catch(Exception ex)
            {
                Log.Error(String.Format("Failed to write local category list for {0}", username), ex);
            }
            finally
            {
                if (file != null)
                    file.Close();
            }
        }
    }
}
