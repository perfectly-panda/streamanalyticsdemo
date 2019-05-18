<script>
import { Line, mixins } from 'vue-chartjs'
const { reactiveProp } = mixins

export default {
  extends: Line,
  components: { Line },
  mixins: [mixins.reactiveProp],
  props: {
    chartData: {
      type: Object,
      default: null
    },
    options: {
      type: Object,
      default: null
    }
  },
  watch: {
  chartData () {
    this.$data._chart.update()
  }
    },
  mounted () {
    this.renderChart(this.chartData, this.options)
  }
}
</script>