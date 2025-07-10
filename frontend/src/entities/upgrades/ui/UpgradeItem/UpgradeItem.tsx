import Image from 'next/image'
import cn from 'classnames'

import { LootItem, LootRarity } from '@/entities/loot'

import classes from './UpgradeItem.module.scss'

type UpgradeItemProps = {
  result: 'success' | 'failure'
}

export const UpgradeItem = ({ result }: UpgradeItemProps) => {
  return (
    <div className={classes.upgradeItem}>
      <div className={classes.titles}>
        <div>
          <p className={classes.columnTitle}>Ваша ставка</p>
        </div>

        <div>
          <p className={classes.columnTitle}>Апгрейд</p>
        </div>
      </div>

      <div className={classes.items}>
        <LootItem
          className={classes.loot}
          rarity={LootRarity.COMMON}
          price={980}
          name='Starned Corbem'
        />

        <Image
          src={
            result === 'success'
              ? '/icons/upgrade-double-success.svg'
              : '/icons/upgrade-double-failure.svg'
          }
          width={26}
          height={26}
          alt='Иконка'
        />

        <LootItem
          className={classes.loot}
          rarity={LootRarity.RARE}
          price={1980}
          name='Starned Corbem'
        />
      </div>

      <div className={classes.upgradeInfo}>
        <div className={classes.chance}>
          <p>
            Шанс: <span className={classes.chanceValue}>93.5%</span>
          </p>
        </div>

        <div
          className={cn(classes.result, {
            [classes.success]: result === 'success',
            [classes.failure]: result === 'failure'
          })}
        >
          <p>{result === 'success' ? 'Выигрыш' : 'Проигрыш'}</p>
        </div>
      </div>
    </div>
  )
}
