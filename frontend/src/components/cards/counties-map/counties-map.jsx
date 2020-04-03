import React from 'react';
import ReactEcharts from 'echarts-for-react';
import { Card } from '../../layout/card/card';
import { mnemonics } from '../../../config/mnemonics';
import { Constants } from '../../../config/globals';

export const EMBED_COUNTIES_MAP = 'counties-map';

export class CountiesMap extends React.PureComponent {
  getChartOptions(counties) {
    const data = counties.map(countie => {
      return {
        name: mnemonics[countie.name],
        value: countie.value
      };
    });
    return {
      tooltip: {
        trigger: 'item'
      },
      visualMap: {
        show: false,
        min: 0,
        max: 0,
        left: 'left',
        top: 'bottom',
        text: ['Ridicat', 'Scazut'],
        calculable: true,
        inRange: {
          color: [Constants.countyLowestColor, Constants.countyHighestColor]
        }
      },
      series: [
        {
          name: 'Cazuri',
          type: 'map',
          mapType: 'RO',
          itemStyle: {
            areaColor: Constants.curedColor
          },
          emphasis: {
            label: {
              show: false
            }
          },
          data
        }
      ]
    };
  }

  render() {
    const { state } = this.props;
    const { isLoaded, error, counties, stale, lastUpdatedOnString } = state;

    return (
      <Card
        error={error}
        isLoaded={isLoaded}
        title="Cazuri confirmate pe judete"
        subtitle={`Ultima actualizare: ${lastUpdatedOnString}`}
        isStale={stale}
        embedPath={EMBED_COUNTIES_MAP}
      >
        {counties && (
          <ReactEcharts
            option={this.getChartOptions(counties)}
            style={{ height: '400px' }}
            className="react_for_echarts"
          />
        )}
      </Card>
    );
  }
}
