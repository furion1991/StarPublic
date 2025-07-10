import { PageActions, PriceWithCurrency } from '@/shared/ui'
import { WinnersHistory } from '@/entities/crash-rocket'
import { CrashRocketMainScreen } from '@/widgets/crash-rocket'

import classes from './CrashRocket.module.scss'

export const CrashRocket = () => {
  return (
    <>
      <PageActions />

      <div className={classes.crashRocketPage}>
        <div className={classes.wrapper}>
          <h1>Crash Rocket</h1>

          <div className={classes.content}>
            <div className={classes.left}>
              <div className={classes.stats}>
                <div className={classes.onlineNumber}>
                  <div className={classes.circle} />

                  <p>
                    Online: <span className={classes.onlineNumberValue}>180</span>
                  </p>

                  <svg
                    xmlns='http://www.w3.org/2000/svg'
                    width='205'
                    height='83'
                    viewBox='0 0 205 83'
                    fill='none'
                    className={classes.statsBg}
                  >
                    <path
                      d='M205 77.9642V5C205 2.23858 202.761 0 200 0H32.3792C23.2426 0 15.2675 6.19172 13.0032 15.0433L0.753204 62.9286C-1.66278 72.3728 5.43021 81.5763 15.1782 81.6458L199.964 82.9641C202.74 82.9839 205 80.7396 205 77.9642Z'
                      fill='#2C327C'
                    />
                  </svg>
                </div>

                <div className={classes.totalBetsValue}>
                  <PriceWithCurrency
                    image={{
                      width: 30,
                      height: 30
                    }}
                  >
                    {new Intl.NumberFormat('de-DE').format(70369)}
                  </PriceWithCurrency>

                  <svg
                    xmlns='http://www.w3.org/2000/svg'
                    width='205'
                    height='83'
                    viewBox='0 0 205 83'
                    fill='none'
                    className={classes.statsBg}
                  >
                    <path
                      d='M0 77.9642V5C0 2.23858 2.23858 0 5.00001 0H172.621C181.757 0 189.732 6.19171 191.997 15.0433L204.247 62.9286C206.663 72.3728 199.57 81.5763 189.822 81.6458L5.03567 82.9641C2.26038 82.9839 0 80.7396 0 77.9642Z'
                      fill='url(#paint0_linear_33_16359)'
                    />
                    <defs>
                      <linearGradient
                        id='paint0_linear_33_16359'
                        x1='104.5'
                        y1='0'
                        x2='104.5'
                        y2='83'
                        gradientUnits='userSpaceOnUse'
                      >
                        <stop stopColor='#373E96' />
                        <stop offset='1' stopColor='#1B6ED2' />
                      </linearGradient>
                    </defs>
                  </svg>
                </div>
              </div>

              <div className={classes.winnersHistory}>
                <WinnersHistory />
              </div>
            </div>

            <CrashRocketMainScreen />
          </div>
        </div>
      </div>
    </>
  )
}
