using MFiles.VAF;
using MFiles.VAF.Common;
using MFiles.VAF.Configuration;
using MFiles.VAF.Core;
using MFilesAPI;
using System;
using System.Diagnostics;

namespace SalesApplication
{
    /// <summary>
    /// The entry point for this Vault Application Framework application.
    /// </summary>
    /// <remarks>Examples and further information available on the developer portal: http://developer.m-files.com/. </remarks>
    public class VaultApplication
        : ConfigurableVaultApplicationBase<Configuration>
    {

        int TotalOrderAmount_ID = 1493;
        double TotalOrderAmount_Value = 0.0;


        int TotalMargin_ID = 1490;
        double TotalMargin_Value = 0.0;


        //int Diff_Price_Support_ID = 1495;
        //double Diff_Price_Support_Value = 0.0;


        //int TotalProjectAmount_ID = 1498;
        //double TotalProjectAmount_Value = 0.0;


        //int EarnestMoney_Amount_ID = 1500;
        //double EarnestMoney_Amount_Value = 0.0;


        //int RetentiojMoneyAmount_ID = 1502;
        //double RetentiojMoneyAmount_Value = 0.0;



        [EventHandler(MFEventHandlerType.MFEventHandlerAfterSetProperties, ObjectType = "OT.ProductDetails")]
        [EventHandler(MFEventHandlerType.MFEventHandlerAfterDeleteObject, ObjectType = "OT.ProductDetails")]
        [EventHandler(MFEventHandlerType.MFEventHandlerBeforeCreateNewObjectFinalize, ObjectType = "OT.ProductDetails")]
        public void CalculatingProductsTotal(EventHandlerEnvironment env)
        {
            try
            {
                

                var ProductOwner = env.PropertyValues.GetProperty(1118).Value.DisplayValue.ToString(); //Owner(Customer Enquiry)

                // Create our search builder.
                var searchBuilder = new MFSearchBuilder(env.Vault);

                // Add an object type filter.
                searchBuilder.ObjType(env.ObjVer.Type);
                searchBuilder.Property(1118, MFDataType.MFDatatypeText, ProductOwner);

                // Add a "not deleted" filter.
                searchBuilder.Deleted(false);

                // Execute the search.
                var searchResults = searchBuilder.FindEx();


                TotalOrderAmount_Value = 0.0;
                TotalMargin_Value = 0.0;
                foreach (var item in searchResults)
                {
                    if (!object.ReferenceEquals(item.GetProperty(1118), null))
                    {


                        //for existing Products
                        var Owner = item.GetProperty(1118).Value.DisplayValue; //Owner(Customer Enquiry)

                        if (Owner == ProductOwner)
                        {
                            var Brand = item.GetProperty(1583).Value.DisplayValue.ToString();
                            var Category = item.GetProperty(1581).Value.DisplayValue.ToString();
                            //var ProductMargin = Convert.ToDouble(item.GetProperty(1593).Value.Value.ToString());
                            //var ProductMargin = Convert.ToDouble(item.GetProperty(1599).Value.Value.ToString());

                            var ProductMargin = Convert.ToDouble(item.GetProperty(1605).Value.Value.ToString());
                            var ProductTotalAmmountIncludingTax_Margin = Convert.ToDouble(item.GetProperty(1604).Value.Value.ToString());

                            TotalMargin_Value += ProductMargin;
                            TotalOrderAmount_Value += ProductTotalAmmountIncludingTax_Margin;
                        }
                    }
                }

               
                var CustomerEnquiryObj = new MFSearchBuilder(env.Vault);

                // Add an object type filter.
                CustomerEnquiryObj.ObjType(124);
                CustomerEnquiryObj.Property(1363, MFDataType.MFDatatypeText, ProductOwner);

                // Add a "not deleted" filter.
                CustomerEnquiryObj.Deleted(false);

                // Execute the search.
                var EnquiryResults = CustomerEnquiryObj.FindEx();

                var ObjType = 0;
                var ID = 0;
                foreach (var item in EnquiryResults)
                {
                    if (!object.ReferenceEquals(item.GetProperty(1363), null))
                    {
                        var CustomerEnquiry_ID = item.GetProperty(1363).Value.DisplayValue;

                        #region Inquiry Fileds Calculation


                        //var Target_Price = Convert.ToDouble(item.GetProperty(1494).Value.DisplayValue);
                        //var Final_Approved_Price = Convert.ToDouble(item.GetProperty(1497).Value.DisplayValue);
                        //var Earnest_Money = Convert.ToDouble(item.GetProperty(1499).Value.DisplayValue);
                        //var Retention_Money = Convert.ToDouble(item.GetProperty(1501).Value.DisplayValue);


                        ////Calculating Diff/Price
                        //Diff_Price_Support_Value = TotalOrderAmount_Value - Target_Price;

                        ////Calculating Total Project Amount
                        //TotalProjectAmount_Value = TotalOrderAmount_Value - Final_Approved_Price;

                        ////Calculating Earnest Money Amount

                        //EarnestMoney_Amount_Value = (TotalProjectAmount_Value * Earnest_Money) / 100;

                        //// Calculating Retention Money For 1 Year Amount

                        //RetentiojMoneyAmount_Value = (TotalProjectAmount_Value * Retention_Money) / 100;


                        #endregion


                        if (CustomerEnquiry_ID == ProductOwner)
                        {
                            ObjType = item.ObjVer.Type;
                            ID = item.ObjVer.ID;
                            break;
                        }
                    }

                }
                // We want to alter the document with ID 249.
                var objID = new MFilesAPI.ObjID();
                objID.SetIDs(
                    ObjType: ObjType,
                    ID: ID);



                // Check out the object.
                var checkedOutObjectVersion = env.Vault.ObjectOperations.CheckOut(objID);

                // Create a property value to update Total Order Amount.
                var p_TotalOrderAmount = new MFilesAPI.PropertyValue
                {
                    PropertyDef = TotalOrderAmount_ID
                };
                p_TotalOrderAmount.Value.SetValue(
                        MFDataType.MFDatatypeFloating,  // This must be correct for the property definition.
                        TotalOrderAmount_Value
                    );

                // Update the property on the server.
                env.Vault.ObjectPropertyOperations.SetProperty(
                    ObjVer: checkedOutObjectVersion.ObjVer,
                    PropertyValue: p_TotalOrderAmount);



                // Create a property value to update Total Margin Amount.
                var p_TotalMargin = new MFilesAPI.PropertyValue
                {
                    PropertyDef = TotalMargin_ID
                };
                p_TotalMargin.Value.SetValue(
                        MFDataType.MFDatatypeFloating,  // This must be correct for the property definition.
                        TotalMargin_Value
                    );

                // Update the property on the server.
                env.Vault.ObjectPropertyOperations.SetProperty(
                    ObjVer: checkedOutObjectVersion.ObjVer,
                    PropertyValue: p_TotalMargin);


                #region Fields calculated in VB Scripts

                // Create a property value to update Diff/Price Suport.
                //var p_DiffPriceSupport = new MFilesAPI.PropertyValue
                //{
                //    PropertyDef = Diff_Price_Support_ID
                //};
                //p_DiffPriceSupport.Value.SetValue(
                //        MFDataType.MFDatatypeFloating,  // This must be correct for the property definition.
                //        Diff_Price_Support_Value
                //    );

                //// Update the property on the server.
                //env.Vault.ObjectPropertyOperations.SetProperty(
                //    ObjVer: checkedOutObjectVersion.ObjVer,
                //    PropertyValue: p_DiffPriceSupport);


                // Create a property value to update Total Project Amount.
                //var p_TotalProjectAmount = new MFilesAPI.PropertyValue
                //{
                //    PropertyDef = TotalProjectAmount_ID
                //};
                //p_TotalProjectAmount.Value.SetValue(
                //        MFDataType.MFDatatypeFloating,  // This must be correct for the property definition.
                //        TotalProjectAmount_Value
                //    );

                //// Update the property on the server.
                //env.Vault.ObjectPropertyOperations.SetProperty(
                //    ObjVer: checkedOutObjectVersion.ObjVer,
                //    PropertyValue: p_TotalProjectAmount);


                // Create a property value to update Earnest Money Amount.
                //var p_EarnestMoneyAmount = new MFilesAPI.PropertyValue
                //{
                //    PropertyDef = EarnestMoney_Amount_ID
                //};
                //p_EarnestMoneyAmount.Value.SetValue(
                //        MFDataType.MFDatatypeFloating,  // This must be correct for the property definition.
                //        EarnestMoney_Amount_Value
                //    );

                //// Update the property on the server.
                //env.Vault.ObjectPropertyOperations.SetProperty(
                //    ObjVer: checkedOutObjectVersion.ObjVer,
                //    PropertyValue: p_EarnestMoneyAmount);



                // Create a property value to update Retention Money Amount.
                //var p_RetentionMoneyAmount = new MFilesAPI.PropertyValue
                //{
                //    PropertyDef = RetentiojMoneyAmount_ID
                //};
                //p_RetentionMoneyAmount.Value.SetValue(
                //        MFDataType.MFDatatypeFloating,  // This must be correct for the property definition.
                //        RetentiojMoneyAmount_Value
                //    );

                //// Update the property on the server.
                //env.Vault.ObjectPropertyOperations.SetProperty(
                //    ObjVer: checkedOutObjectVersion.ObjVer,
                //    PropertyValue: p_RetentionMoneyAmount);

                #endregion

                // Check the object back in.
                env.Vault.ObjectOperations.CheckIn(checkedOutObjectVersion.ObjVer);
            }

            catch (Exception ex)
            {


            }

        }


   
    }
}