import Image from 'next/image'
import cn from 'classnames'

import { Ufo } from '../Ufo/Ufo'

import type { UfoColor, UpgradeLootItem } from '../../types/upgrades.types'

import classes from './UfoItemHolder.module.scss'

type UfoItemHolderProps = {
  item: UpgradeLootItem | null
  color: UfoColor
  glowing: boolean
  direction: 'left' | 'right'
  ItemSellSlot?: React.ReactNode
}

export const UfoItemHolder = ({
  item,
  direction,
  color,
  glowing,
  ItemSellSlot
}: UfoItemHolderProps) => {
  return (
    <>
      <Ufo color={color} direction={direction} glowing={glowing} />

      <div className={cn(classes.itemHolder, classes[direction])}>
        {item ? (
          <>
            <div className={classes.upgradeItemImage}>
              <Image src={item.image} width={200} height={200} alt={item.name} />
            </div>

            <p className={classes.itemName}>{item.name}</p>

            {ItemSellSlot}
          </>
        ) : (
          <>
            <div className={classes.placeholderImage}>
              <Image
                src='/img/upgrades/upgrade-gun-placeholder.svg'
                width={163}
                height={143}
                alt='Пистолет'
              />
            </div>

            <p>Выберите предмет с вашего инвентаря</p>
          </>
        )}
      </div>
    </>
  )
}
