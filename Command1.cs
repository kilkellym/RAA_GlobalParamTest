namespace RAA_GlobalParamTest
{
    [Transaction(TransactionMode.Manual)]
    public class Command1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Revit application and document variables
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            //Reference curRef = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);
            //Element curElem = doc.GetElement(curRef);

            //Parameter param1 = curElem.LookupParameter("Top Constraint");
            //Parameter param2 = curElem.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE);


            // get global parameters
            string paramName = "Test";
            string paramName2 = "Test 2";
            GlobalParameter gParam1 = GetGlobalParameterByName(doc, paramName);
            GlobalParameter gParam2 = GetGlobalParameterByName(doc, paramName2);

            if(gParam1 == null)
            {
                using (Transaction t1 = new Transaction(doc))
                {
                    t1.Start("Add global parameter");
                    // create the parameter
#if REVIT2020 || REVIT2021
                    GlobalParameter.Create(doc, paramName, ParameterType.Number);
#else
                    GlobalParameter.Create(doc, paramName, SpecTypeId.Number);
#endif
                    t1.Commit();

                    gParam1 = GetGlobalParameterByName(doc, paramName);
                }
            }

            if(gParam2 == null)
            {
                using (Transaction t1 = new Transaction(doc))
                {
                    t1.Start("Add global parameter2");
                    // create the parameter
#if REVIT2020 || REVIT2021
                    GlobalParameter.Create(doc, paramName2, ParameterType.Number);
#else
                    GlobalParameter.Create(doc, paramName2, SpecTypeId.Number);
#endif
                    t1.Commit();

                    gParam2 = GetGlobalParameterByName(doc, paramName2);
                }
            }
            
            double value = GetGlobalParamValueDouble(gParam1);
            double value2 = GetGlobalParamValueDouble(gParam2);

            // set global parameter value
            using (Transaction t2 = new Transaction(doc))
            {
                t2.Start("Set Global Parameter Value");
                SetGlobalParamValue(gParam1, 123445);
                SetGlobalParamValue(gParam2, 20.45);
                t2.Commit();
            }

            return Result.Succeeded;
        }

        public static GlobalParameter GetGlobalParameterByName(Document document, String name)
        {
            GlobalParameter returnParam = null;
            if (GlobalParametersManager.AreGlobalParametersAllowed(document))
            {
                ElementId paramId = GlobalParametersManager.FindByName(document, name);
                returnParam = document.GetElement(paramId) as GlobalParameter;
            }

            return returnParam;
        }

        public static string GetGlobalParamValueString(GlobalParameter param)
        {
            ParameterValue paramValue = param.GetValue();
            if (paramValue is StringParameterValue stringValue)
                return stringValue.Value;
            else
                return string.Empty;
        }

        public static double GetGlobalParamValueDouble(GlobalParameter parameter)
        {
            ParameterValue paramValue = parameter.GetValue();
            if (paramValue is DoubleParameterValue doubleValue)
            {
                return doubleValue.Value;
            }
            return 0;
        }

        public static void SetGlobalParamValue(GlobalParameter parameter, string value)
        {
            StringParameterValue paramValue = new StringParameterValue(value);
            parameter.SetValue(paramValue);
        }

        public static void SetGlobalParamValue(GlobalParameter parameter, double value)
        {
            DoubleParameterValue paramValue = new DoubleParameterValue(value);
            parameter.SetValue(paramValue);
        }

        internal static PushButtonData GetButtonData()
        {
            // use this method to define the properties for this command in the Revit ribbon
            string buttonInternalName = "btnCommand1";
            string buttonTitle = "Button 1";

            Common.ButtonDataClass myButtonData = new Common.ButtonDataClass(
                buttonInternalName,
                buttonTitle,
                MethodBase.GetCurrentMethod().DeclaringType?.FullName,
                Properties.Resources.Blue_32,
                Properties.Resources.Blue_16,
                "This is a tooltip for Button 1");

            return myButtonData.Data;
        }

    }

}
