var fromLanguageCode='';
var toLanguageCode = '';

function captureLanguageCodes() {
    const fromLanguageCodeElement = document.getElementById('FromLanguageCode');
    const toLanguageCodeElement = document.getElementById('ToLanguageCode');
    if (fromLanguageCodeElement && toLanguageCodeElement) {
        fromLanguageCode=fromLanguageCodeElement.value;
        toLanguageCode=toLanguageCodeElement.value;
    }
}
function flipLanguageCodes() {
    const fromLanguageCodeElement = document.getElementById('FromLanguageCode');
    const toLanguageCodeElement = document.getElementById('ToLanguageCode');
    if (fromLanguageCodeElement && toLanguageCodeElement) {
        toLanguageCodeElement.value=fromLanguageCode;
        fromLanguageCodeElement.value=toLanguageCode;
        let otherLanguageCode = fromLanguageCode;
        fromLanguageCode=toLanguageCode;
        toLanguageCode=otherLanguageCode;
    } 
}
