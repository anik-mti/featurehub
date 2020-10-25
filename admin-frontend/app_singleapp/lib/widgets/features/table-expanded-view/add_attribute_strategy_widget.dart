import 'package:flutter/material.dart';
import 'package:flutter/rendering.dart';
import 'package:mrapi/api.dart';

class AttributeStrategyWidget extends StatefulWidget {
  final RolloutStrategyAttribute attribute;

  final  attributeStrategyFieldName;

  const AttributeStrategyWidget({
    Key key, this.attribute, this.attributeStrategyFieldName,

  }) :  super(key: key);


  @override
  _AttributeStrategyWidgetState createState() => _AttributeStrategyWidgetState();
}

class _AttributeStrategyWidgetState extends State<AttributeStrategyWidget> {
  final TextEditingController _customAttributeKey = TextEditingController();
  final TextEditingController _customAttributeValue = TextEditingController();

  RolloutStrategyAttributeConditional _dropDownCustomAttributeMatchingCriteria;
  bool isUpdate = false;
  String attributeStrategyType;

  _AttributeStrategyWidgetState();

  @override
  void initState() {
    super.initState();
    if (widget.attribute != null) {
      _dropDownCustomAttributeMatchingCriteria = widget.attribute.conditional;
      isUpdate = true;
      attributeStrategyType = widget.attribute.fieldName;
    }
    else {
      attributeStrategyType = widget.attributeStrategyFieldName;
    }
  }

  @override
  Widget build(BuildContext context) {
    return Row(
      mainAxisAlignment: MainAxisAlignment.spaceEvenly,
      crossAxisAlignment: CrossAxisAlignment.center,
      mainAxisSize: MainAxisSize.max,
      children: [
        if(attributeStrategyType == 'country') Text('Country')
        else if (attributeStrategyType == 'device') Text ('Device')
        else Flexible(
            child:  TextFormField(
                controller: _customAttributeKey,
                decoration: InputDecoration(
                    labelText: 'Custom attribute key',
                    helperText:
                    'E.g. userId'),
                // readOnly: !widget.widget.editable,
                autofocus: true,
                onFieldSubmitted: (_) => FocusScope.of(context).nextFocus(),
                // inputFormatters: [
                //   DecimalTextInputFormatter(
                //       decimalRange: 4, activatedNegativeValues: false)
                // ],
                validator: ((v) {
                  if (v.isEmpty) {
                    return 'Attribute key required';
                  }
                  return null;
                })),
          ),
        Spacer(),
        InkWell(
          mouseCursor: SystemMouseCursors.click,
          child: DropdownButton(
            icon: Padding(
              padding: EdgeInsets.only(left: 8.0),
              child: Icon(
                Icons.keyboard_arrow_down,
                size: 24,
              ),
            ),
            isExpanded: false,
            items: RolloutStrategyAttributeConditional.values
                .map((RolloutStrategyAttributeConditional dropDownStringItem) {
              return DropdownMenuItem<RolloutStrategyAttributeConditional>(
                  value: dropDownStringItem,
                  child: Text(
                      _transformValuesToString(dropDownStringItem),
                      style: Theme.of(context).textTheme.bodyText2));
            }).toList(),

            hint: Text('Select condition',
                style: Theme.of(context).textTheme.subtitle2),
            onChanged: (value) {
              var readOnly = true;//TODO parametrise this if needed
              if (!readOnly) {
                setState(() {
                  _dropDownCustomAttributeMatchingCriteria = value;
                });
              }
            },
            value: _dropDownCustomAttributeMatchingCriteria,
          ),
        ),
        Spacer(),
        Flexible(
          child: TextFormField(
              controller: _customAttributeValue,
              decoration: InputDecoration(
                  labelText: 'Custom attribute value(s)',
                  helperText:
                  'E.g. bob@xyz.com, mary@xyz.com'),
              // readOnly: !widget.widget.editable,
              autofocus: true,
              onFieldSubmitted: (_) => FocusScope.of(context).nextFocus(),
              // inputFormatters: [
              //   DecimalTextInputFormatter(
              //       decimalRange: 4, activatedNegativeValues: false)
              // ],
              validator: ((v) {
                if (v.isEmpty) {
                  return 'Attribute value(s) required';
                }
                return null;
              })),
        ),
      ],
    );
  }

  String _transformValuesToString(RolloutStrategyAttributeConditional dropDownStringItem) {
    switch (dropDownStringItem) {
      case RolloutStrategyAttributeConditional.EQUALS:
        return 'equals';
      case RolloutStrategyAttributeConditional.NOT_EQUALS:
        return 'not equals';
      case RolloutStrategyAttributeConditional.ENDS_WITH:
        return 'ends with';
      case RolloutStrategyAttributeConditional.STARTS_WITH:
        return 'starts with';
      case RolloutStrategyAttributeConditional.GREATER:
        return 'greater';
        break;
      case RolloutStrategyAttributeConditional.GREATER_EQUALS:
        return 'greater or equals';
        break;
      case RolloutStrategyAttributeConditional.LESS:
        return 'less';
        break;
      case RolloutStrategyAttributeConditional.LESS_EQUALS:
        return 'less or equals';
        break;
      case RolloutStrategyAttributeConditional.INCLUDES:
        return 'includes';
        break;
      case RolloutStrategyAttributeConditional.EXCLUDES:
        return 'excludes';
        break;
      case RolloutStrategyAttributeConditional.REGEX:
        return 'regex';
        break;
    }

    return '';
  }


}
