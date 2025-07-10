import Link from 'next/link'

import { UpgradeItem } from '@/entities/upgrades'
import { Button, NoDataPanel } from '@/shared/ui'

import type { UpgradeItem as UpgradeItemType } from '@/entities/user'

import classes from './UpgradesHistoryList.module.scss'

type UpgradesHistoryListProps = {
  upgrades: UpgradeItemType[]
}

export const UpgradesHistoryList = ({ upgrades }: UpgradesHistoryListProps) => {
  if (!upgrades.length) {
    return (
      <NoDataPanel
        title='Нет апгрейдов'
        text='Создайте свой первый апгрейд'
        action={
          <Link href='/upgrades'>
            <Button>Апгрейды ›</Button>
          </Link>
        }
      />
    )
  }

  return (
    <ul className={classes.upgradesHistoryList}>
      {upgrades
        .sort((a, b) => Number(new Date(b.dateOfUpgrade)) - Number(new Date(a.dateOfUpgrade)))
        .map((upgradeItemProps) => {
          return (
            <li key={upgradeItemProps.dateOfUpgrade}>
              <UpgradeItem {...upgradeItemProps} />
            </li>
          )
        })}
    </ul>
  )
}
