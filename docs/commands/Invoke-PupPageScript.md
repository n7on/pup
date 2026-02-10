---
external help file: Pup.dll-Help.xml
Module Name:
online version:
schema: 2.0.0
---

# Invoke-PupPageScript

## SYNOPSIS
Executes JavaScript code in the page context.

## SYNTAX

## DESCRIPTION
Runs arbitrary JavaScript in the browser page context.
Can return values of various types.
Powerful for DOM manipulation, extracting data, and testing XSS scenarios.

## EXAMPLES

### Example 1: Extract CSRF token
```
$csrf = Invoke-PupPageScript -Page $page -Script "() => document.querySelector('input[name=csrf]').value" -AsString
```

Extracts a CSRF token from a hidden form field.

### Example 2: Get all form data
```
$formData = Invoke-PupPageScript -Page $page -Script @"
() => {
    const form = document.querySelector('form');
    const data = new FormData(form);
    return Object.fromEntries(data.entries());
}
"@
```

Extracts all form field names and values.

### Example 3: Test DOM-based XSS
```
Invoke-PupPageScript -Page $page -Script "() => { document.body.innerHTML += '<img src=x onerror=alert(1)>'; }" -AsVoid
```

Injects content to test for DOM-based XSS.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
