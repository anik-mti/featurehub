import 'package:app_singleapp/widgets/features/custom_strategy_bloc.dart';
import 'package:app_singleapp/widgets/features/table-expanded-view/number/edit_number_value_container.dart';
import 'package:app_singleapp/widgets/features/table-expanded-view/strategies/strategy_card_widget.dart';
import 'package:flutter/material.dart';
import 'package:mrapi/api.dart';

class NumberStrategyCard extends StatelessWidget {
  final RolloutStrategy rolloutStrategy;
  final CustomStrategyBloc strBloc;

  NumberStrategyCard({Key key, this.rolloutStrategy, this.strBloc})
      : super(key: key);

  @override
  Widget build(BuildContext context) {
    return StreamBuilder<bool>(
        stream: strBloc.fvBloc
            .environmentIsLocked(strBloc.environmentFeatureValue.environmentId),
        builder: (ctx, snap) {
          if (snap.hasData) {
            final canChangeValue = strBloc.environmentFeatureValue.roles
                .contains(RoleType.CHANGE_VALUE);
            var editable = !snap.data && canChangeValue;
            final enabled = editable && !snap.data;
            return StrategyCardWidget(
              editable: editable,
              strBloc: strBloc,
              rolloutStrategy: rolloutStrategy,
              editableHolderWidget: EditNumberValueContainer(
                canEdit: editable,
                enabled: enabled,
                rolloutStrategy: rolloutStrategy,
                strBloc: strBloc,
              ),
            );
          } else {
            return SizedBox.shrink();
          }
        });
  }
}
