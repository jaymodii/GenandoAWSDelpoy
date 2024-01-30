export class SystemConstant {
  public static post = 'POST';
  public static delete = 'DELETE';
  public static put = 'PUT';
  public static authorization = 'authorization';
  public static expectedRole = 'expectedRole';
  public static testPrefix = 'TST00';
}

export class HeaderTabConstant {
  public static dashboard = 'Dashboard';
  public static patients = 'Patients';
  public static lab = 'Lab';
  public static faq = 'FAQ';
  public static contactUs = 'Contact Us';
  public static contactDoctor = 'Contact Doctor';
}

export class LinksTitleConstant {
  public static createNewPatient = 'Create a New Patient';
  public static contactUs = 'Contact To Technical Assistant';
  public static testExplaination = 'Test Explaination';
  public static faq = 'FAQ';
  public static testMeans = 'What does your test mean';
  public static createQuestions = 'Add / Edit General Questions';
}

export class UserRole {
  public static doctor = 'doctor';
  public static patient = 'patient';
  public static lab = 'lab';

  public static doctorRoleId = '1';
  public static patientRoleId = '2';
  public static labRoleId = '3';
}

export class DropdownClinicalPrecessStep {
  public static contactGenando = 'Contact Genando';
  public static prescribeTest = 'Prescribe Test';
  public static collectSample = 'Collect Sample';
  public static shipSample = 'Ship Sample';
  public static seeLabResult = 'See Lab Result';
  public static reqMoreInfo = 'Request More Info';
  public static publishReport = 'Publish Report';
  public static receiveSample = 'Receive Sample';
  public static uploadResult = 'Upload Result';
  public static updateResult = 'Update Result';
  public static Report = 'See Report';
  public static complete = 'Complete';
  public static seeClinicalPath = 'See Clinical Path';
  public static clinicalPath = 'Clinical Path';
  public static seeRequestedInfo = 'See Requested Info';
  public static addClinicalQuestions = 'Add Clinical Questions';
  public static waitForPatient = 'Wait For Patient';
  public static waitForLab = 'Wait For Lab';
  public static initial = 'Initial';
  public static sampleAnalysis = 'Sample Analysis';
  public static sendLabResults = 'Send Lab Results';
}

export class QuestionTypeConstant {
  public static textValue = 'text';
  public static textViewValue = 'Text';
  public static radioValue = 'radio';
  public static radioViewValue = 'Radio';
  public static checkboxValue = 'checkbox';
  public static checkboxViewValue = 'Checkbox';
  public static selectValue = 'select';
  public static selectViewValue = 'Select';
}
