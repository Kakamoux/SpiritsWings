using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Security.Permissions;
using System.Net.Security;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections;

namespace serveur
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
     [ServiceContract(Namespace = "xnaReseau",
        CallbackContract = typeof(IServeurRappel),
        SessionMode = SessionMode.Required, ProtectionLevel = ProtectionLevel.None)]
    public interface IServeur
    {

        [OperationContract(IsInitiating = true, IsTerminating = false, IsOneWay = true, ProtectionLevel = ProtectionLevel.None)]
         void DemarrerSession(String pseudo);

        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = false, ProtectionLevel = ProtectionLevel.None)]
        Vector3 move(Vector3 xwingPosition, Quaternion xwingRotation, float moveSpeed);

        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true, ProtectionLevel = ProtectionLevel.None)]
        void sychronizeRotation(Quaternion xwingRotation,String pseudo);

           [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true, ProtectionLevel = ProtectionLevel.None)]
        void sychronizePosition(Vector3 xwingPosition, String pseudo);
        
        

        // TODO: Add your service operations here
          [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = false, ProtectionLevel = ProtectionLevel.None)]
         void newBullet(double p, Vector3 xwingPosition, Quaternion xwingRotation, string pseudo);

          [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = false, ProtectionLevel = ProtectionLevel.None)]
          void updateBullet(Vector3 vector3, Quaternion quaternion, string pseudo, int i);

          [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = false, ProtectionLevel = ProtectionLevel.None)]
          void removeBullet(int i, String pseudo);

         [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = false, ProtectionLevel = ProtectionLevel.None)]
          List<bulletReturn> getBullets(String pseudo);

         [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = false, ProtectionLevel = ProtectionLevel.None)]
         List<aircraftReturn> getAircrafts(String pseudo);

         [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true, ProtectionLevel = ProtectionLevel.None)]
         void touched(int i, String pseudoCible, String pseudoAttaquant);

          [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true, ProtectionLevel = ProtectionLevel.None)]
         void penalitate(String pseudo);
    }

     [DataContract(IsReference=true)]
     public class aircraftReturn
     {
         [DataMember]
         public Vector3 position;
          [DataMember]
         public Quaternion rotation;
          [DataMember]
         public String pseudo;
          [DataMember]
          public int score;
         

     }

     [DataContract(IsReference = true)]
     public class bulletReturn
     {
         [DataMember]
         public Vector3 position;
         [DataMember]
         public Quaternion rotation;
         [DataMember]
         public String pseudo;


     }

    




    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    
}
